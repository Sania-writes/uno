﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MUXControlsTestApp.Utilities;
using Private.Infrastructure;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using FluentAssertions.Execution;
using Windows.UI;

namespace Uno.UI.RuntimeTests.Tests.Windows_UI_Xaml_Controls
{
	[TestClass]
	public class Given_Grid
	{
		[TestMethod]
		[DataRow(20, 20, 70, 60, 0, 0, 90, 20, 110, 0, 30, 20, 160, 0, 90, 20, 0, 40, 90, 260, 110, 40, 30, 120, 160,
			40, 90, 120, 110, 180, 140, 120)]
		[DataRow(20, 80, 70, 180, 0, 0, 90, 20, 110, 0, 30, 20, 160, 0, 90, 20, 0, 100, 90, 200, 110, 100, 30, 60, 160,
			100, 90, 60, 110, 240, 140, 60)]
		[DataRow(80, 80, 190, 180, 0, 0, 30, 20, 110, 0, 30, 20, 220, 0, 30, 20, 0, 100, 30, 200, 110, 100, 30, 60, 220,
			100, 30, 60, 110, 240, 140, 60)]
		[DataRow(-20, 20, 0, 60, 0, 0, 130, 20, 110, 0, 30, 20, 120, 0, 130, 20, 0, 40, 130, 260, 110, 40, 30, 120, 120,
			40, 130, 120, 110, 180, 140, 120)]
		[DataRow(20, -20, 70, 0, 0, 0, 90, 20, 110, 0, 30, 20, 160, 0, 90, 20, 0, 0, 90, 300, 110, 0, 30, 160, 160, 0,
			90, 160, 110, 140, 140, 160)]
		[DataRow(-20, -20, 0, 0, 0, 0, 130, 20, 110, 0, 30, 20, 120, 0, 130, 20, 0, 0, 130, 300, 110, 0, 30, 160, 120,
			0, 130, 160, 110, 140, 140, 160)]
		public async Task When_Has_ColumnSpacing(double columnSpacing,
			double rowSpacing,
			double gridDesiredWidthExpected, double gridDesiredHeightExpected,
			double child0LeftExpected, double child0TopExpected, double child0WidthExpected,
			double child0HeightExpected,
			double child1LeftExpected, double child1TopExpected, double child1WidthExpected,
			double child1HeightExpected,
			double child2LeftExpected, double child2TopExpected, double child2WidthExpected,
			double child2HeightExpected,
			double child3LeftExpected, double child3TopExpected, double child3WidthExpected,
			double child3HeightExpected,
			double child4LeftExpected, double child4TopExpected, double child4WidthExpected,
			double child4HeightExpected,
			double child5LeftExpected, double child5TopExpected, double child5WidthExpected,
			double child5HeightExpected,
			double child6LeftExpected, double child6TopExpected, double child6WidthExpected, double child6HeightExpected
		)
		{
			await RunOnUIThread.Execute(() =>
			{
				TestServices.WindowHelper.WindowContent = null;
			});

			Grid SUT = null;
			await RunOnUIThread.Execute(() =>
			{
				SUT = new Grid
				{
					ColumnSpacing = columnSpacing,
					RowSpacing = rowSpacing,
					ColumnDefinitions =
					{
						new ColumnDefinition {Width = GridLengthHelper.FromValueAndType(1, GridUnitType.Star)},
						new ColumnDefinition {Width = GridLengthHelper.FromValueAndType(1, GridUnitType.Auto)},
						new ColumnDefinition {Width = GridLengthHelper.FromValueAndType(1, GridUnitType.Star)},
					},
					RowDefinitions =
					{
						new RowDefinition {Height = GridLengthHelper.FromValueAndType(1, GridUnitType.Auto)},
						new RowDefinition {Height = GridLengthHelper.FromValueAndType(1, GridUnitType.Star)},
						new RowDefinition {Height = GridLengthHelper.FromValueAndType(1, GridUnitType.Star)},
					},
					Children =
					{
						GetChild(0, 0, height: 20),
						GetChild(1, 0, width: 30, height: 20),
						GetChild(2, 0, height: 20),
						GetChild(0, 1, rowSpan: 2),
						GetChild(1, 1, width: 30),
						GetChild(2, 1),
						GetChild(1, 2, colSpan: 2)
					}
				};

				var outerBorder = new Border {Width = 250, Height = 300};
				outerBorder.Child = SUT;

				TestServices.WindowHelper.WindowContent = outerBorder;

				FrameworkElement GetChild(int gridCol, int gridRow, int? colSpan = null, int? rowSpan = null,
					double? width = null, double? height = null)
				{
					var child = new Border();

					Grid.SetColumn(child, gridCol);
					Grid.SetRow(child, gridRow);
					if (colSpan.HasValue)
					{
						Grid.SetColumnSpan(child, colSpan.Value);
					}

					if (rowSpan.HasValue)
					{
						Grid.SetRowSpan(child, rowSpan.Value);
					}

					if (width.HasValue)
					{
						child.Width = width.Value;
					}
					else
					{
						child.HorizontalAlignment = HorizontalAlignment.Stretch;
					}

					if (height.HasValue)
					{
						child.Height = height.Value;
					}
					else
					{
						child.VerticalAlignment = VerticalAlignment.Stretch;
					}

					return child;
				}
			});

			await WaitForMeasure(SUT);

			await RunOnUIThread.Execute(() =>
			{
				var desiredSize = SUT.DesiredSize;
				var data = $"({columnSpacing}, {rowSpacing}, {desiredSize.Width}, {desiredSize.Height}";
				foreach (var child in SUT.Children)
				{
					var layoutRect = LayoutInformation.GetLayoutSlot(child as FrameworkElement);
					data += $", {layoutRect.Left}, {layoutRect.Top}, {layoutRect.Width}, {layoutRect.Height}";
				}

				data += ")";
				Debug.WriteLine(data);

				Assert.AreEqual(new Size(gridDesiredWidthExpected, gridDesiredHeightExpected), desiredSize);

#if !__ANDROID__ && !__IOS__ // These assertions fail on Android/iOS because layout slots aren't set the same way as UWP
				var layoutRect0Actual = LayoutInformation.GetLayoutSlot(SUT.Children[0] as FrameworkElement);
				var layoutRect0Expected = new Rect(child0LeftExpected, child0TopExpected, child0WidthExpected,
					child0HeightExpected);
				Assert.AreEqual(layoutRect0Expected, layoutRect0Actual);

				var layoutRect1Actual = LayoutInformation.GetLayoutSlot(SUT.Children[1] as FrameworkElement);
				var layoutRect1Expected = new Rect(child1LeftExpected, child1TopExpected, child1WidthExpected,
					child1HeightExpected);
				Assert.AreEqual(layoutRect1Expected, layoutRect1Actual);

				var layoutRect2Actual = LayoutInformation.GetLayoutSlot(SUT.Children[2] as FrameworkElement);
				var layoutRect2Expected = new Rect(child2LeftExpected, child2TopExpected, child2WidthExpected,
					child2HeightExpected);
				Assert.AreEqual(layoutRect2Expected, layoutRect2Actual);

				var layoutRect3Actual = LayoutInformation.GetLayoutSlot(SUT.Children[3] as FrameworkElement);
				var layoutRect3Expected = new Rect(child3LeftExpected, child3TopExpected, child3WidthExpected,
					child3HeightExpected);
				Assert.AreEqual(layoutRect3Expected, layoutRect3Actual);

				var layoutRect4Actual = LayoutInformation.GetLayoutSlot(SUT.Children[4] as FrameworkElement);
				var layoutRect4Expected = new Rect(child4LeftExpected, child4TopExpected, child4WidthExpected,
					child4HeightExpected);
				Assert.AreEqual(layoutRect4Expected, layoutRect4Actual);

				var layoutRect5Actual = LayoutInformation.GetLayoutSlot(SUT.Children[5] as FrameworkElement);
				var layoutRect5Expected = new Rect(child5LeftExpected, child5TopExpected, child5WidthExpected,
					child5HeightExpected);
				Assert.AreEqual(layoutRect5Expected, layoutRect5Actual);

				var layoutRect6Actual = LayoutInformation.GetLayoutSlot(SUT.Children[6] as FrameworkElement);
				var layoutRect6Expected = new Rect(child6LeftExpected, child6TopExpected, child6WidthExpected,
					child6HeightExpected);
				Assert.AreEqual(layoutRect6Expected, layoutRect6Actual);
#endif

				TestServices.WindowHelper.WindowContent = null;
			});
		}

		private async Task WaitForMeasure(FrameworkElement view, int timeOutMs = 1000)
		{
			var isMeasured = false;
			var stopwatch = Stopwatch.StartNew();
			while (stopwatch.ElapsedMilliseconds < timeOutMs)
			{
				await RunOnUIThread.Execute(() =>
				{
					isMeasured = view.DesiredSize != default(Size);
				});
				if (isMeasured)
				{
					return;
				}

				await Task.Delay(50);
			}
		}

		private static SolidColorBrush Gray = new SolidColorBrush { Color = Colors.Gray };
		private static SolidColorBrush Blue = new SolidColorBrush { Color = Colors.Blue };
		private static SolidColorBrush White = new SolidColorBrush { Color = Colors.White };
		private static SolidColorBrush Red = new SolidColorBrush { Color = Colors.Red };

		[TestMethod]
		[RunsOnUIThread]
		public async Task When_MeasuredInfinite_AndStarColumns()
		{
			var sut = new Grid {BorderBrush = Gray, BorderThickness = new Thickness(5)};

			sut.ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Auto});
			sut.ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.FromString("*")});
			sut.ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.FromString("2*")});

			var border1 = new Border {MinWidth = 100, MinHeight = 160, Background = Blue};
			var border2 = new Border {MinWidth = 100, MinHeight = 160, Background = White};
			var border3 = new Border {MinWidth = 100, MinHeight = 160, Background = Red};

			Grid.SetColumn(border1, 0);
			Grid.SetColumn(border2, 1);
			Grid.SetColumn(border3, 2);

			sut.Children.Add(border1);
			sut.Children.Add(border2);
			sut.Children.Add(border3);

			var canvas = new Canvas();
			canvas.Children.Add(sut);

			TestServices.WindowHelper.WindowContent = canvas;

			await TestServices.WindowHelper.WaitForIdle();

			using (new AssertionScope())
			{
				sut.DesiredSize.Should().BeOfWidth(310, because: "desired width")
					.And.BeOfHeight(170, because: "desired height");
				border1.DesiredSize.Should().BeOfWidth(100);
				border2.DesiredSize.Should().BeOfWidth(100);
				border3.DesiredSize.Should().BeOfWidth(100);

				sut.RenderSize.Should().BeOfWidth(310, because: "actual width")
					.And.BeOfHeight(170, because: "actual height");
				border1.RenderSize.Should().BeOfWidth(100);
				border2.RenderSize.Should().BeOfWidth(100);
				border3.RenderSize.Should().BeOfWidth(100);
			}
		}


		[TestMethod]
		[RunsOnUIThread]
		public async Task When_MeasuredInfinite_AndStarRows()
		{
			var sut = new Grid {BorderBrush = Gray, BorderThickness = new Thickness(5)};

			sut.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});
			sut.RowDefinitions.Add(new RowDefinition {Height = GridLength.FromString("*")});
			sut.RowDefinitions.Add(new RowDefinition {Height = GridLength.FromString("2*")});

			var border1 = new Border {MinWidth = 160, MinHeight = 100, Background = Blue};
			var border2 = new Border {MinWidth = 160, MinHeight = 100, Background = White};
			var border3 = new Border {MinWidth = 160, MinHeight = 100, Background = Red};

			Grid.SetRow(border1, 0);
			Grid.SetRow(border2, 1);
			Grid.SetRow(border3, 2);

			sut.Children.Add(border1);
			sut.Children.Add(border2);
			sut.Children.Add(border3);

			var canvas = new Canvas();
			canvas.Children.Add(sut);

			TestServices.WindowHelper.WindowContent = canvas;

			await TestServices.WindowHelper.WaitForIdle();

			using (new AssertionScope())
			{
				sut.DesiredSize.Should().BeOfWidth(170, because: "desired width")
					.And.BeOfHeight(310, because: "desired height");
				border1.DesiredSize.Should().BeOfHeight(100, because: "d1");
				border2.DesiredSize.Should().BeOfHeight(100, because: "d2");
				border3.DesiredSize.Should().BeOfHeight(100, because: "d3");

				sut.RenderSize.Should().BeOfWidth(170, because: "actual width")
					.And.BeOfHeight(310, because: "actual height");
				border1.RenderSize.Should().BeOfHeight(100, because: "r1");
				border2.RenderSize.Should().BeOfHeight(100, because: "r2");
				border3.RenderSize.Should().BeOfHeight(100, because: "r3");
			}
		}
	}
}

