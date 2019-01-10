﻿using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Cheats.Reflection
{
	/// <summary>
	/// Represents binary operator.
	/// </summary>
	public enum BinaryOperator
	{
		/// <summary>
		/// An addition operation, such as a + b, without overflow checking.
		/// </summary>
		Add = ExpressionType.Add,

		/// <summary>
		/// An addition operation, such as (a + b), with overflow checking.
		/// </summary>
		AddChecked = ExpressionType.AddChecked,
		
		/// <summary>
		/// An subtraction operation, such as (a - b), without overflow checking.
		/// </summary>
		Subtract = ExpressionType.Subtract,

		/// <summary>
		/// A subtraction operation, such as (a - b), with overflow checking.
		/// </summary>
		SubtractChecked = ExpressionType.SubtractChecked,

		/// <summary>
		/// a &amp; b
		/// </summary>
		And = ExpressionType.And,
		
		/// <summary>
		/// a | b
		/// </summary>
		Or = ExpressionType.Or,

		/// <summary>
		/// a / b
		/// </summary>
		Divide = ExpressionType.Divide,

		/// <summary>
		/// A multiply operation, such as (a * b), without overflow checking.
		/// </summary>
		Multiply = ExpressionType.Multiply,
		
		/// <summary>
		/// a &lt;&lt; b
		/// </summary>
		LeftShift = ExpressionType.LeftShift,
		
		/// <summary>
		/// a &gt;&gt; b
		/// </summary>
		RightShift = ExpressionType.RightShift,

		/// <summary>
		/// A multiply operation, such as (a * b), with overflow checking.
		/// </summary>
		MultiplyChecked = ExpressionType.MultiplyChecked,
		
		/// <summary>
		/// a % b
		/// </summary>
		Modulo = ExpressionType.Modulo,

		/// <summary>
		/// a == b
		/// </summary>
		Equal = ExpressionType.Equal,

		/// <summary>
		/// a != b
		/// </summary>
		NotEqual = ExpressionType.NotEqual,

		/// <summary>
		/// a ^ b
		/// </summary>
		Xor = ExpressionType.ExclusiveOr,

		/// <summary>
		/// a &gt; b
		/// </summary>
		GreaterThan = ExpressionType.GreaterThan,

		/// <summary>
		/// a &gt;= b
		/// </summary>
		GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual,

		/// <summary>
		/// a &lt; b
		/// </summary>
		LessThan = ExpressionType.LessThan,

		/// <summary>
		/// a &lt;= b
		/// </summary>
		LessThanOrEqual = ExpressionType.LessThanOrEqual,

		/// <summary>
		/// POW(a, b)
		/// </summary>
		Power = ExpressionType.Power
	}

	public sealed class BinaryOperator<T, OP, R>: Operator<Operator<T, OP, R>>
	{
		private BinaryOperator(Expression<Operator<T, OP, R>> invoker, BinaryOperator type, MethodInfo overloaded)
			: base(invoker.Compile(), type.ToExpressionType(), overloaded)
		{
		}

		/// <summary>
		/// Invokes binary operator.
		/// </summary>
		/// <param name="first">First operand.</param>
		/// <param name="second">Second operand.</param>
		/// <returns>Result of binary operator.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public R Invoke(in T first, in OP second) => invoker(in first, in second);

		/// <summary>
        /// Type of operator.
        /// </summary>
        public new BinaryOperator Type => (BinaryOperator)base.Type;

		private static Expression<Operator<T, OP, R>> MakeBinary(Operator.Kind @operator, Operator.Operand first, Operator.Operand second, out MethodInfo overloaded)
		{
			var resultType = typeof(R);
			//perform automatic cast from byte/short/ushort/sbyte so binary operators become available for these types
			var usePrimitiveCast = resultType.IsPrimitive && first.NormalizePrimitive() && second.NormalizePrimitive();
			tail_call:	//C# doesn't support tail calls so replace it with label/goto
			overloaded = null;
			try
			{
				var body =  @operator.MakeBinary(first, second);
				overloaded = body.Method;
				return overloaded is null && usePrimitiveCast ?
					Expression.Lambda<Operator<T, OP, R>>(Expression.Convert(body, resultType), first.Source, second.Source) :
					Expression.Lambda<Operator<T, OP, R>>(body, first.Source, second.Source);
			} 
			catch(ArgumentException e)
			{
				Debug.WriteLine(e);
				return null;
			}
			catch(InvalidOperationException)
			{
				if(second.Upcast())
					goto tail_call;
				else if(first.Upcast())
				{
					second = second.Source;
					goto tail_call;
				}
				else
					return null;
			}
		}

		internal static BinaryOperator<T, OP, R> Reflect(Operator.Kind op)
		{
			var first = Expression.Parameter(typeof(T).MakeByRefType(), "first");
			var second = Expression.Parameter(typeof(OP).MakeByRefType(), "second");
			var expr = MakeBinary(op, first, second, out var overloaded);
			return expr is null ? null : new BinaryOperator<T, OP, R>(expr, op, overloaded);
		}
	}
}
