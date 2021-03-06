﻿using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class InsertTest : ParseTestBase
	{
		[Fact]
		public void Insert_table_values_1()
		{
			GivenText(@"INSERT [dbo].[customer] ([custid]) VALUES 
(267467)
");
			WhenParse(SqlParser.InsertExpr);
			ThenResultShouldBe(new SqlInsertExpression()
			{
				Table = new SqlObjectNameExpression()
				{
					Name = "[dbo].[customer]",
				},
				Fields = new SqlBaseFieldExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "[custid]"
					},
				},
				InsertRows = new[]
				{
					new SqlInsertRowExpression()
					{
						Values = new SqlExpression[]
						{
							new SqlNumberExpression()
							{
								Value = 267467,
								ValueTypeFullname = typeof(int).FullName
							},
						}
					}
				}
			});
		}

		[Fact]
		public void Insert_table_values_null()
		{
			GivenText(@"INSERT [dbo].[customer] ([custid]) VALUES 
(NULL)
");
			WhenParse(SqlParser.InsertExpr);
			ThenResultShouldBe(new SqlInsertExpression()
			{
				Table = new SqlObjectNameExpression()
				{
					Name = "[dbo].[customer]",
				},
				Fields = new SqlBaseFieldExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "[custid]"
					},
				},
				InsertRows = new[]
				{
					new SqlInsertRowExpression()
					{
						Values = new SqlExpression[]
						{
							new SqlNullExpression(),
						}
					}
				}
			});
		}

		[Fact]
		public void Insert_table_values_cast()
		{
			GivenText(@"INSERT [dbo].[customer] ([custid]) VALUES 
(CAST(0x0000A5E5006236FB AS DateTime))
");
			WhenParse(SqlParser.InsertExpr);
			ThenResultShouldBe(new SqlInsertExpression()
			{
				Table = new SqlObjectNameExpression()
				{
					Name = "[dbo].[customer]",
				},
				Fields = new SqlBaseFieldExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "[custid]"
					},
				},
				InsertRows = new[]
				{
					new SqlInsertRowExpression()
					{
						Values = new SqlExpression[]
						{
							new SqlFunctionExpression()
							{
								Name = "CAST",
								Parameters = new SqlExpression[]
								{
									new SqlHexExpression()
									{
										HexStr = "0000A5E5006236FB"
									},
									new SqlDataTypeExpression()
									{
										DataType = "DateTime"
									}
								}
							},
						}
					}
				}
			});
		}


		[Fact]
		public void Insert_table_values_1_nstr_cast()
		{
			GivenText(@"INSERT [dbo].[customer] ([custid], [firstname], [lastname], [birth]) VALUES 
(267467, N'', NULL, CAST(0x0000A5E5006236FB AS DateTime))
");
			WhenParse(SqlParser.InsertExpr);
			ThenResultShouldBe(new SqlInsertExpression()
			{
				Table = new SqlObjectNameExpression()
				{
					Name = "[dbo].[customer]",
				},
				Fields = new SqlBaseFieldExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "[custid]"
					},
					new SqlTableFieldExpression()
					{
						Name = "[firstname]"
					},
					new SqlTableFieldExpression()
					{
						Name = "[lastname]"
					},
					new SqlTableFieldExpression()
					{
						Name = "[birth]"
					}
				},
				InsertRows = new[]
				{
					new SqlInsertRowExpression()
					{
						Values = new SqlExpression[]
						{
							new SqlNumberExpression()
							{
								Value = 267467,
								ValueTypeFullname = typeof(int).FullName
							},
							new SqlStringExpression()
							{
								IsUnicode = true,
								Text = "",
							},
							new SqlNullExpression(),
							new SqlFunctionExpression()
							{
								Name = "CAST",
								Parameters = new SqlExpression[]
								{
									new SqlHexExpression()
									{
										HexStr = "0000A5E5006236FB"
									},
									new SqlDataTypeExpression()
									{
										DataType = "DateTime"
									}
								}
							}
						}
					}
				}
			});
		}

		[Fact]
		public void Insert_into_table_values_1()
		{
			GivenText(@"INSERT INTO [dbo].[customer] ([custid], [firstname], [lastname], [birth]) VALUES 
(267467, N'', NULL, CAST(0x0000A5E5006236FB AS DateTime))
");
			WhenParse(SqlParser.InsertExpr);
			ThenResultShouldBe(new SqlInsertExpression()
			{
				HasInto = true,
				Table = new SqlObjectNameExpression()
				{
					Name = "[dbo].[customer]",
				},
				Fields = new SqlBaseFieldExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "[custid]"
					},
					new SqlTableFieldExpression()
					{
						Name = "[firstname]"
					},
					new SqlTableFieldExpression()
					{
						Name = "[lastname]"
					},
					new SqlTableFieldExpression()
					{
						Name = "[birth]"
					}
				},
				InsertRows = new[]
				{
					new SqlInsertRowExpression()
					{
						Values = new SqlExpression[]
						{
							new SqlNumberExpression()
							{
								Value = 267467,
								ValueTypeFullname = typeof(int).FullName
							},
							new SqlStringExpression()
							{
								IsUnicode = true,
								Text = "",
							},
							new SqlNullExpression(),
							new SqlFunctionExpression()
							{
								Name = "CAST",
								Parameters = new SqlExpression[]
								{
									new SqlHexExpression()
									{
										HexStr = "0000A5E5006236FB"
									},
									new SqlDataTypeExpression()
									{
										DataType = "DateTime"
									}
								}
							}
						}
					}
				}
			});
		}

		[Fact]
		public void Insert_table_field_values_cast_negative_numeric()
		{
			GivenText(
				"INSERT [dbo].[TimeZones] ([Name]) VALUES (CAST(-12.00 AS Numeric(4, 2)))");
			WhenParse(SqlParser.InsertExpr);
			ThenResultShouldBe(new SqlInsertExpression()
			{
				Table = new SqlObjectNameExpression()
				{
					Name = "[dbo].[TimeZones]"
				},
				Fields = new SqlBaseFieldExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "[Name]"
					}
				},
				InsertRows = new []
				{
					new SqlInsertRowExpression()
					{
						Values = new SqlExpression[]
						{
							new SqlFunctionExpression()
							{
								Name = "CAST",
								Parameters = new SqlExpression[]
								{
									new SqlNumberExpression()
									{
										Value = -12.0d,
										ValueTypeFullname = typeof(decimal).FullName
									},
									new SqlDataTypeExpression()
									{
										DataType = "Numeric",
										Size = 4,
										Scale = 2
									}
								}
							}
						}
					}
				}
			});
		}
	}
}