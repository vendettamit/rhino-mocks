using System;
using System.Reflection;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Expectations
{
	/// <summary>
	/// Expect the method's arguments to match the contraints
	/// </summary>
	public class ConstraintsExpectation : AbstractExpectation
	{
		private AbstractConstraint[] constraints;

		/// <summary>
		/// Creates a new <see cref="ConstraintsExpectation"/> instance.
		/// </summary>
		/// <param name="method">Method.</param>
		/// <param name="constraints">Constraints.</param>
		public ConstraintsExpectation(MethodInfo method, AbstractConstraint[] constraints) : base(method)
		{
			Validate.IsNotNull(constraints, "constraints");
			this.constraints = constraints;
			ConstraintsMatchMethod();
		}

		/// <summary>
		/// Creates a new <see cref="ConstraintsExpectation"/> instance.
		/// </summary>
		/// <param name="expectation">Expectation.</param>
		/// <param name="constraints">Constraints.</param>
		public ConstraintsExpectation(IExpectation expectation, AbstractConstraint[] constraints) : base(expectation)
		{
			Validate.IsNotNull(constraints, "constraints");
			this.constraints = constraints;
			ConstraintsMatchMethod();
		}

		/// <summary>
		/// Validate the arguments for the method.
		/// </summary>
		/// <param name="args">The arguments with which the method was called</param>
		protected override bool DoIsExpected(object[] args)
		{
			Validate.IsNotNull(args, "args");
			if (args.Length != constraints.Length)
				throw new InvalidOperationException("Number of argument doesn't match the number of parameters!");
			for (int i = 0; i < args.Length; i++)
			{
				if (constraints[i].Eval(args[i]) == false)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value></value>
		public override string ErrorMessage
		{
			get
			{
				MethodCallUtil.FormatArgumnet format = new MethodCallUtil.FormatArgumnet(FormatArgWithConstraint);
				string stringPresentation = MethodCallUtil.StringPresentation(format, Method, constraints);
				return CreateErrorMessage(stringPresentation);
			}
		}

		private void ConstraintsMatchMethod()
		{
            if (constraints.Length != Method.GetParameters().Length)
				throw new InvalidOperationException("The number of constraints is not the same as the number of the method's parameters!");
			for (int i = 0; i < constraints.Length; i++)
			{
				if (constraints[i] == null)
					throw new InvalidOperationException(string.Format("The constraint at index {0} is null! Use Is.Null() to represent null parameters.", i));
			}
		}

		private string FormatArgWithConstraint(Array args, int i)
		{
			return constraints[i].Message;
		}

		/// <summary>
		/// Determines if the object equal to expectation
		/// </summary>
		public override bool Equals(object obj)
		{
			ConstraintsExpectation other = obj as ConstraintsExpectation;
			if (other == null)
				return false;
            return Method.Equals(other.Method) && Validate.ArgsEqual(constraints, other.constraints);
		}

		/// <summary>
		/// Get the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}