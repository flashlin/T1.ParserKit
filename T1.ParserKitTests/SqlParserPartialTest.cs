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
