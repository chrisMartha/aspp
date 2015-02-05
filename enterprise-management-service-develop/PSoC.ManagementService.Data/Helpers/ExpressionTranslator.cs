using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace PSoC.ManagementService.Data.Helpers
{
    public class ExpressionTranslator : ExpressionVisitor
    {
        private StringBuilder sb;
        private string _orderBy = string.Empty;
        private int? _skip = null;
        private int? _take = null;
        private string _whereClause = string.Empty;

        #region Constructors

        public ExpressionTranslator()
        {
        }

        #endregion Constructors

        public string OrderBy
        {
            get
            {
                return _orderBy;
            }
        }

        public int? Skip
        {
            get
            {
                return _skip;
            }
        }

        public int? Take
        {
            get
            {
                return _take;
            }
        }

        public string WhereClause
        {
            get
            {
                return _whereClause;
            }
        }

        public string TranslateToSql(Expression expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);

            if (this.sb.Length > 0)
                _whereClause = "WHERE " + this.sb.ToString();

            if (string.IsNullOrWhiteSpace(_orderBy) == false)
            {
                _orderBy = (string.IsNullOrWhiteSpace(_whereClause) ? "" : " ") + "ORDER BY " + _orderBy;
            }

            return _whereClause + _orderBy;
        }

        protected bool IsNullConstant(Expression exp)
        {
            if (exp is MemberExpression)
            {
                var m = (MemberExpression)exp;
                if (m.Member is FieldInfo)
                {
                    var container = ((ConstantExpression)m.Expression).Value;
                    var value = ((FieldInfo)m.Member).GetValue(container);

                    return value == null;
                }

            }

            return (exp is ConstantExpression && ((ConstantExpression)exp).Value == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS ");
                    }
                    else
                    {
                        sb.Append(" = ");
                    }
                    break;

                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS NOT ");
                    }
                    else
                    {
                        sb.Append(" <> ");
                    }
                    break;

                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;

                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

            }

            this.Visit(b.Right);
            sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q == null && c.Value == null)
            {
                sb.Append("NULL");
            }
            else if (q == null)
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)c.Value) ? 1 : 0);
                        break;

                    case TypeCode.String:
                    case TypeCode.Char:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.DateTime:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));

                    default:
                        sb.Append(c.Value);
                        break;
                }
            }

            return c;
        }

        protected Expression VisitLike(MethodCallExpression m)
        {
            this.VisitMember(m.Object as MemberExpression);
            sb.Append(" LIKE ");
            var c = m.Arguments[0] as ConstantExpression;

            switch (m.Method.Name)
            {
                case "Contains":
                    sb.Append("'%");
                    sb.Append(c.Value);
                    sb.Append("%'");
                    break;
                case "StartsWith":
                    sb.Append("'");
                    sb.Append(c.Value);
                    sb.Append("'%");
                    break;
                case "EndsWith":
                    sb.Append("'");
                    sb.Append(c.Value);
                    sb.Append("%'");
                    break;
            }

            return m;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                var parm = m.Expression as ParameterExpression;
                sb.Append(parm.Name + '.');
                sb.Append(m.Member.Name);
                return m;
            }

            if (m.Member.Name == "Empty")
            {
                Type t = m.Member.DeclaringType;
                sb.Append(GetDefaultValue(t));

                return m;
            }

            if (m.Expression is ConstantExpression &&
                    m.Member is FieldInfo)
            {
                object container =
                    ((ConstantExpression)m.Expression).Value;
                object value = ((FieldInfo)m.Member).GetValue(container);
                if (value != null)
                    sb.Append("'" + value.ToString() + "'");
                else
                    sb.Append("NULL");
                return m;
            }

            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Constant)
            {
                var c = m.Expression as ConstantExpression;
                this.Visit(c);
                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Where")
            {
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                return m;
            }

            switch (m.Method.Name)
            {
                case "Take":
                    if (this.ParseTakeExpression(m))
                    {
                        Expression nextExpression = m.Arguments[0];
                        return this.Visit(nextExpression);
                    }
                    break;
                case "Skip":
                    if (this.ParseSkipExpression(m))
                    {
                        Expression nextExpression = m.Arguments[0];
                        return this.Visit(nextExpression);
                    }
                    break;
                case "OrderBy":
                    if (this.ParseOrderByExpression(m, "ASC"))
                    {
                        return m;
                    }
                    break;
                case "OrderByDescending":
                    if (this.ParseOrderByExpression(m, "DESC"))
                    {
                        return m;
                    }
                    break;
                case "Any":
                    LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    this.Visit(lambda.Body);
                    return m;
                case "Contains":
                case "StartsWith":
                case "EndsWith":
                    return this.VisitLike(m);
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        private string GetDefaultValue(Type t)
        {
            if (!t.IsValueType || Nullable.GetUnderlyingType(t) != null)
                return "NULL";

            return "'" + Activator.CreateInstance(t).ToString() + "'";
        }

        private bool ParseOrderByExpression(MethodCallExpression expression, string order)
        {
            if (expression.Arguments[0] is MethodCallExpression)
            {
                MethodCallExpression nextExpression = expression.Arguments[0] as MethodCallExpression;
                this.Visit(nextExpression);
            }

            LambdaExpression lambdaExpression = (LambdaExpression)expression.Arguments[1];
            MemberExpression body = lambdaExpression.Body as MemberExpression;

            if (body != null)
            {
                var parm = body.Expression as ParameterExpression;

                if (string.IsNullOrEmpty(_orderBy))
                {
                    _orderBy = string.Format("{0}.{1} {2}", parm.Name, body.Member.Name, order);
                }
                else
                {
                    _orderBy = string.Format("{0}, {1}{2} {3}", _orderBy, parm.Name, body.Member.Name, order);
                }

                return true;
            }

            return false;
        }

        private bool ParseSkipExpression(MethodCallExpression expression)
        {
            ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if (int.TryParse(sizeExpression.Value.ToString(), out size))
            {
                _skip = size;
                return true;
            }

            return false;
        }

        private bool ParseTakeExpression(MethodCallExpression expression)
        {
            ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if (int.TryParse(sizeExpression.Value.ToString(), out size))
            {
                _take = size;
                return true;
            }

            return false;
        }
    }
}