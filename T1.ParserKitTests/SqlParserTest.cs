﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserTest
	{
		private string _code;
		private ITextSpan[] _result;
		private SqlParser _parser;

		[Fact]
		public void Select_field()
		{
			GiveText("select name from customer");
			WhenParse();
			ThenResultShouldBe(new SelectExpression()
			{
				File = "",
				Length = 25,
				Position = 0,
				Content = _code,
				Fields = new FieldsExpression()
				{
					File = "",
					Length = 4,
					Position = 7,
					Content = _code,
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
							File = "",
							Length = 4,
							Position = 7,
							Content = _code
						}
					}
				},
				From = new TableExpression()
				{
					File = "",
					Length = 8,
					Position = 17,
					Content = _code,
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_variable_assign_field()
		{
			GiveText("select @name=name from customer");
			WhenParse();
			ThenResultShouldBe(new SelectExpression()
			{
				File = "",
				Length = _code.Length,
				Position = 0,
				Content = _code,
				Fields = new FieldsExpression()
				{
					File = "",
					Length = 10,
					Position = 7,
					Content = _code,
					Items = new List<SqlExpression>()
					{
						new VariableAssignFieldExpression()
						{
							VariableName = "@name",
							Name = "name",
							File = "",
							Length = 10,
							Position = 7,
							Content = _code
						},
					}
				},
				From = new TableExpression()
				{
					File = "",
					Length = 8,
					Position = 23,
					Content = _code,
					Name = "customer"
				}
			});
		}



		[Fact]
		public void Select_field_from_table_nolock()
		{
			GiveText("select name from customer with(nolock)");
			WhenParse();
			ThenResultShouldBe(new SelectExpression()
			{
				File = "",
				Length = _code.Length,
				Position = 0,
				Content = _code,
				Fields = new FieldsExpression()
				{
					File = "",
					Length = 4,
					Position = 7,
					Content = _code,
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
							File = "",
							Length = 4,
							Position = 7,
							Content = _code
						}
					}
				},
				From = new TableExpression()
				{
					File = "",
					Length = 21,
					Position = 17,
					Content = _code,
					Name = "customer",
					WithOption = new WithOptionExpression()
					{
						File = "",
						Length = 12,
						Position = 26,
						Content = _code,
						Nolock = true
					}
				}
			});
		}

		[Fact]
		public void Select_tb_field()
		{
			GiveText("select c.name from customer c");
			WhenParse();
			ThenResultShouldBe(new SelectExpression()
			{
				File = "",
				Length = _code.Length,
				Position = 0,
				Content = _code,
				Fields = new FieldsExpression()
				{
					File = "",
					Length = 6,
					Position = 7,
					Content = _code,
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							File = "",
							Length = 6,
							Position = 7,
							Content = _code,
							Name = "name",
							From = "c"
						}
					}
				},
				From = new TableExpression()
				{
					File = "",
					Length = 10,
					Position = 19,
					Content = _code,
					Name = "customer",
					AliasName = "c"
				}
			});
		}

		[Fact]
		public void Select_tb_field_alias()
		{
			GiveText("select c.name username from customer c");
			WhenParse();
			ThenResultShouldBe(new SelectExpression()
			{
				File = "",
				Length = _code.Length,
				Position = 0,
				Content = _code,
				Fields = new FieldsExpression()
				{
					File = "",
					Length = 15,
					Position = 7,
					Content = _code,
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							File = "",
							Length = 15,
							Position = 7,
							Content = _code,
							Name = "name",
							From = "c",
							AliasName = "username"
						}
					}
				},
				From = new TableExpression()
				{
					File = "",
					Length = 10,
					Position = 28,
					Content = _code,
					Name = "customer",
					AliasName = "c"
				}
			});
		}

		[Fact]
		public void Select_Field_alias()
		{
			GiveText("select name n1 from customer");
			WhenParse();
			ThenResultShouldBe(new SelectExpression()
			{
				File = "",
				Length = 28,
				Position = 0,
				Content = "select name n1 from customer",
				Fields = new FieldsExpression()
				{
					File = "",
					Length = 7,
					Position = 7,
					Content = "select name n1 from customer",
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
							AliasName = "n1",
							File = "",
							Length = 7,
							Position = 7,
							Content = "select name n1 from customer"
						}
					}
				},
				From = new TableExpression()
				{
					File = "",
					Length = 8,
					Position = 20,
					Content = "select name n1 from customer",
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_Field_as_alias()
		{
			GiveText("select name as n1 from customer");
			WhenParse();
			ThenResultShouldBe(new SelectExpression()
			{
				File = "",
				Length = 31,
				Position = 0,
				Content = _code,
				Fields = new FieldsExpression()
				{
					File = "",
					Length = 10,
					Position = 7,
					Content = _code,
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							File = "",
							Length = 10,
							Position = 7,
							Content = _code,
							Name = "name",
							AliasName = "n1"
						}
					}
				},
				From = new TableExpression()
				{
					File = "",
					Length = 8,
					Position = 23,
					Content = _code,
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_Field_as_alias_from_table_as_alias()
		{
			GiveText("select name as n1 from customer as c");
			WhenParse();
			ThenResultShouldBe(new SelectExpression()
			{
				File = "",
				Length = 36,
				Position = 0,
				Content = _code,
				Fields = new FieldsExpression()
				{
					File = "",
					Length = 10,
					Position = 7,
					Content = _code,
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							File = "",
							Length = 10,
							Position = 7,
							Content = _code,
							Name = "name",
							AliasName = "n1"
						}
					}
				},
				From = new TableExpression()
				{
					File = "",
					Length = 13,
					Position = 23,
					Content = _code,
					Name = "customer",
					AliasName = "c"
				}
			});
		}

		[Fact]
		public void Select_Field_as_alias_from_select_name()
		{
			GiveText("select name as n1 from (select name1 from customer) c");
			WhenParse();
			ThenResultShouldBe(new SelectExpression()
			{
				File = "",
				Length = 53,
				Position = 0,
				Content = _code,
				Fields = new FieldsExpression()
				{
					File = "",
					Length = 10,
					Position = 7,
					Content = _code,
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							File = "",
							Length = 10,
							Position = 7,
							Content = _code,
							Name = "name",
							AliasName = "n1"
						}
					}
				},
				From = new SourceExpression()
				{
					File = "",
					Length = 30,
					Position = 23,
					Content = _code,
					Item = new SelectExpression()
					{
						File = "",
						Length = 26,
						Position = 24,
						Content = _code,
						Fields = new FieldsExpression()
						{
							File = "",
							Length = 5,
							Position = 31,
							Content = _code,
							Items = new List<SqlExpression>()
							{
								new FieldExpression()
								{
									File = "",
									Length = 5,
									Position = 31,
									Content = _code,
									Name = "name1"
								}
							}
						},
						From = new TableExpression()
						{
							File = "",
							Length = 8,
							Position = 42,
							Content = _code,
							Name = "customer"
						}
					},
					AliasName = "c"
				}
			});
		}


		private void ThenResultShouldBe(SqlExpression expression)
		{
			expression.ToExpectedObject()
				.ShouldMatch(_result[0]);
		}

		private void WhenParse()
		{
			_parser = new SqlParser();
			_result = _parser.ParseText(_code).Cast<ITextSpan>().ToArray();
		}

		private void GiveText(string code)
		{
			_code = code;
		}
	}
}
