using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DotNext.Linq.Expressions
{
    /// <summary>
    /// Represents structured exception handling statement.
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/try-catch">try-catch statement</seealso>
    public sealed class TryBuilder : ExpressionBuilder<TryExpression>
    {
        public delegate Expression Filter(ParameterExpression exception);
        public delegate Expression Handler(ParameterExpression exception);

        private readonly Expression tryBlock;
        private Expression faultBlock;
        private Expression finallyBlock;
        private readonly ICollection<CatchBlock> handlers;

        internal TryBuilder(Expression tryBlock)
        {
            this.tryBlock = tryBlock;
            faultBlock = finallyBlock = null;
            handlers = new LinkedList<CatchBlock>();
        }

        internal TryBuilder Catch(ParameterExpression exception, Expression filter, Expression handler)
        {
            VerifyCaller();
            handlers.Add(MakeCatchBlock(exception.Type, exception, handler, filter));
            return this;
        }

        /// <summary>
        /// Constructs exception handling section.
        /// </summary>
        /// <param name="exceptionType">Expected exception.</param>
        /// <param name="filter">Additional filter to be applied to the caught exception.</param>
        /// <param name="handler">Exception handling block.</param>
        /// <returns><see langword="this"/> builder.</returns>
        public TryBuilder Catch(Type exceptionType, Filter filter, Handler handler)
        {
            var exception = Variable(exceptionType, "e");
            return Catch(exception, filter?.Invoke(exception), handler(exception));
        }

        /// <summary>
        /// Constructs exception handling section.
        /// </summary>
        /// <param name="exceptionType">Expected exception.</param>
        /// <param name="handler">Exception handling block.</param>
        /// <returns><see langword="this"/> builder.</returns>
        public TryBuilder Catch(Type exceptionType, Handler handler) => Catch(exceptionType, null, handler);

        /// <summary>
        /// Constructs exception handling section.
        /// </summary>
        /// <typeparam name="E">Expected exception.</typeparam>
        /// <param name="handler">Exception handling block.</param>
        /// <returns><see langword="this"/> builder.</returns>
        public TryBuilder Catch<E>(Handler handler) where E : Exception => Catch(typeof(E), handler);

        public TryBuilder Catch(Expression handler) => Catch(Variable(typeof(Exception), "e"), null, handler);

        /// <summary>
        /// Associates expression to be returned from structured exception handling block 
        /// in case of any exception.
        /// </summary>
        /// <param name="fault">The expression to be returned from SEH block.</param>
        /// <returns><see langword="this"/> builder.</returns>
        public TryBuilder Fault(Expression fault)
        {
            VerifyCaller();
            faultBlock = fault;
            return this;
        }

        /// <summary>
        /// Constructs block of code run when control leaves a <see langword="try"/> statement.
        /// </summary>
        /// <param name="finally">The block of code to be executed.</param>
        /// <returns><see langword="this"/> builder.</returns>
        public TryBuilder Finally(Action @finally) => Finally(builder(@finally));

        /// <summary>
        /// Constructs single expression run when control leaves a <see langword="try"/> statement.
        /// </summary>
        /// <param name="finally">The single expression to be executed.</param>
        /// <returns><see langword="this"/> builder.</returns>
        public TryBuilder Finally(Expression @finally)
        {
            VerifyCaller();
            finallyBlock = @finally;
            return this;
        }

        private protected override TryExpression Build() => Expression.MakeTry(ExpressionType, tryBlock, finallyBlock, faultBlock, handlers);
    }
}