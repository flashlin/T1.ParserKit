//		[Fact]
//		public void Where_name_eq_1()
//		{
//			GiveText("where name = 1");
//			WhenTryParse(_parser.WhereExpr);
//			ThenResultShouldBe(new WhereExpression()
//			{
//				File = "",
//				Content = _text,
//				Length = _text.Length,
//				Position = 0,
//				Filter = new FilterExpression()
//				{
//					File = "",
//					Content = _text,
//					Length = 8,
//					Position = 6,
//					Left = new FieldExpression()
//					{
//						File = "",
//						Content = _text,
//						Length = 4,
//						Position = 6,
//						Name = "name"
//					},
//					Oper = "=",
//					Right = new NumberExpression()
//					{
//						File = "",
//						Content = _text,
//						Length = 1,
//						Position = 13,
//						ValueTypeFullname = typeof(int).FullName,
//						Value = 1
//					}
//				}
//			});
//		}


//		[Fact]
//		public void Arithmetic_1_add_2_mul_3_add_4()
//		{
//			GiveText("1 + 2 * 3 + 4");
//			WhenTryParse(_parser.ArithmeticOperatorExpr());
//			ThenResultShouldBe(new ArithmeticOperatorExpression()
//			{
//				File = "",
//				Content = _text,
//				Length = _text.Length,
//				Position = 0,
//				Oper = "+",
//				Left = new ArithmeticOperatorExpression
//				{
//					Left = new NumberExpression
//					{
//						Value = 1,
//						ValueTypeFullname = typeof(int).FullName,
//						File = "",
//						Length = 1,
//						Position = 0,
//						Content = _text
//					},
//					Oper = "+",
//					Right = new ArithmeticOperatorExpression
//					{
//						Left = new NumberExpression
//						{
//							Value = 2,
//							ValueTypeFullname = typeof(int).FullName,
//							File = "",
//							Length = 1,
//							Position = 4,
//							Content = _text
//						},
//						Oper = "*",
//						Right = new NumberExpression
//						{
//							Value = 3,
//							ValueTypeFullname = typeof(int).FullName,
//							File = "",
//							Length = 1,
//							Position = 8,
//							Content = _text
//						},
//						File = "",
//						Length = 5,
//						Position = 4,
//						Content = _text
//					},
//					File = "",
//					Length = 9,
//					Position = 0,
//					Content = _text
//				},
//				Right = new NumberExpression
//				{
//					Value = 4,
//					ValueTypeFullname = typeof(int).FullName,
//					File = "",
//					Length = 1,
//					Position = 12,
//					Content = _text
//				}
//			});
//		}

//		[Fact]
//		public void Arithmetic_Group_LParen_1_add_2_RParen_mul_3()
//		{
//			GiveText("(1 + 2) * 3");
//			WhenTryParse(_parser.ArithmeticOperatorExpr());
//			ThenResultShouldBe(new ArithmeticOperatorExpression()
//			{
//				File = "",
//				Content = _text,
//				Length = _text.Length,
//				Position = 0,
//				Left = new ArithmeticOperatorExpression
//				{
//					File = "",
//					Length = 7,
//					Position = 0,
//					Content = _text,
//					Left = new NumberExpression
//					{
//						Value = 1,
//						ValueTypeFullname = typeof(int).FullName,
//						File = "",
//						Length = 1,
//						Position = 1,
//						Content = _text
//					},
//					Oper = "+",
//					Right = new NumberExpression
//					{
//						Value = 2,
//						ValueTypeFullname = typeof(int).FullName,
//						File = "",
//						Length = 1,
//						Position = 5,
//						Content = _text
//					},
//				},
//				Oper = "*",
//				Right = new NumberExpression
//				{
//					Value = 3,
//					ValueTypeFullname = typeof(int).FullName,
//					File = "",
//					Length = 1,
//					Position = 10,
//					Content = _text
//				}
//			});
//		}


