namespace LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

public static class ConsoleTable
{
	// ── Box Drawing Characters ──
	// Outer borders (double lines)
	private const char Dh = '═';
	private const char Dv = '║';
	private const char DTl = '╔';
	private const char DTr = '╗';
	private const char DBl = '╚';
	private const char DBr = '╝';

	// Inner borders (single lines)
	private const char Sh = '─';
	private const char Sv = '│';

	// Intersections (outer double + inner single)
	private const char Mid = '╤';
	private const char BMid = '╧';
	private const char LMidS = '╟';
	private const char RMidS = '╢';
	private const char LMidD = '╠';
	private const char RMidD = '╣';
	private const char CrossS = '┼';
	private const char CrossD = '╪';


	// Prints a key-value style table perfect for entity details. Values can contain multiple lines separated by \n or as string arrays.
	public static void PrintKeyValueTable(string title, List<(string Label, string[] ValueLines)> rows,
		int labelWidth = 18, int valueWidth = 55)
	{
		int[] widths = [labelWidth, valueWidth];
		var innerWidth = widths.Sum() + 1; // +1 for inner vertical border

		// ── Top Border ──
		Console.Write(DTl);
		Console.Write(new string(Dh, innerWidth));
		Console.WriteLine(DTr);

		// ── Title Row ──
		Console.Write(Dv);
		Console.Write(CenterText(title, innerWidth));
		Console.WriteLine(Dv);

		// ── Header Separator ──
		Console.Write(LMidD);
		Console.Write(new string(Dh, widths[0]));
		Console.Write(Mid);
		Console.Write(new string(Dh, widths[1]));
		Console.WriteLine(RMidD);

		// ── Data Rows ──
		for (var i = 0; i < rows.Count; i++)
		{
			var (label, valueLines) = rows[i];
			var labelLines = new[] { label };
			PrintMultiLineRow(labelLines, valueLines, widths, isLastRow: i == rows.Count - 1);
		}

		// ── Bottom Border ──
		Console.Write(DBl);
		Console.Write(new string(Dh, widths[0]));
		Console.Write(BMid);
		Console.Write(new string(Dh, widths[1]));
		Console.WriteLine(DBr);
	}


	/// Prints a generic multi-column table with headers. Each cell can contain multiple lines.
	public static void PrintTable(string title, string[] headers, List<string[][]> rows)
	{
		var colCount = headers.Length;

		// Calculate column widths based on content
		var widths = new int[colCount];
		for (var c = 0; c < colCount; c++)
		{
			var maxLen = headers[c].Length;
			maxLen = rows.Select(row => row[c].Max(line => line?.Length ?? 0)).Prepend(maxLen).Max();

			widths[c] = maxLen + 2; // +2 for padding
		}

		var innerWidth = widths.Sum() + (colCount - 1); // Sum + inner borders

		// ── Top Border ──
		DrawHorizontalBorder(DTl, DTr, Dh, Dh, widths);

		// ── Title ──
		Console.Write(Dv);
		Console.Write(CenterText(title, innerWidth));
		Console.WriteLine(Dv);

		// ── Header Separator ──
		DrawHorizontalBorder(LMidD, RMidD, Mid, Dh, widths);

		// ── Headers ──
		Console.Write(Dv);
		for (var c = 0; c < colCount; c++)
		{
			Console.Write(CenterText(headers[c], widths[c]));
			if (c < colCount - 1) Console.Write(Sv);
		}

		Console.WriteLine(Dv);

		// ── Header/Data Separator ──
		DrawHorizontalBorder(LMidD, RMidD, CrossD, Dh, widths);

		// ── Data Rows ──
		for (var r = 0; r < rows.Count; r++)
		{
			// Find max lines in this row
			var maxLines = rows[r].Max(cell => cell?.Length ?? 1);
			if (maxLines == 0) maxLines = 1;

			for (var line = 0; line < maxLines; line++)
			{
				Console.Write(Dv);
				for (var c = 0; c < colCount; c++)
				{
					var cellLines = rows[r][c];
					var text = line < cellLines.Length ? cellLines[line] : "";
					Console.Write(PadCell(text, widths[c]));
					if (c < colCount - 1) Console.Write(Sv);
				}

				Console.WriteLine(Dv);
			}

			// Row separator (unless last row)
			if (r < rows.Count - 1) DrawHorizontalBorder(LMidS, RMidS, CrossS, Sh, widths);
		}

		// ── Bottom Border ──
		DrawHorizontalBorder(DBl, DBr, BMid, Dh, widths);
	}


	// ── Helpers ──


	private static void PrintMultiLineRow(string[] leftLines, string[] rightLines, int[] widths, bool isLastRow)
	{
		var maxLines = Math.Max(leftLines.Length, rightLines.Length);

		for (var i = 0; i < maxLines; i++)
		{
			var left = i < leftLines.Length ? leftLines[i] : "";
			var right = i < rightLines.Length ? rightLines[i] : "";

			Console.Write(Dv);
			Console.Write(PadCell(left, widths[0]));
			Console.Write(Sv);
			Console.Write(PadCell(right, widths[1]));
			Console.WriteLine(Dv);
		}

		if (!isLastRow)
		{
			Console.Write(LMidS);
			Console.Write(new string(Sh, widths[0]));
			Console.Write(CrossS);
			Console.Write(new string(Sh, widths[1]));
			Console.WriteLine(RMidS);
		}
	}


	private static void DrawHorizontalBorder(char left, char right, char mid, char horizontal, int[] widths)
	{
		Console.Write(left);
		for (var i = 0; i < widths.Length; i++)
		{
			Console.Write(new string(horizontal, widths[i]));
			if (i < widths.Length - 1) Console.Write(mid);
		}

		Console.WriteLine(right);
	}


	private static string PadCell(string text, int width)
	{
		// Ensures exactly 1 space padding on left, fills rest with spaces
		if (text.Length > width - 2) text = text[..(width - 2)];
		return $" {text}".PadRight(width);
	}


	private static string CenterText(string text, int width)
	{
		if (string.IsNullOrEmpty(text)) return new string(' ', width);
		if (text.Length > width - 2) text = text[..(width - 2)];

		var totalPadding = width - text.Length;
		var leftPad = totalPadding / 2;
		var rightPad = totalPadding - leftPad;

		return new string(' ', leftPad) + text + new string(' ', rightPad);
	}


	public static string[] WrapText(string text, int maxWidth)
	{
		if (string.IsNullOrWhiteSpace(text)) return ["—"];

		text = text.Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
		var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		var lines = new List<string>();
		var current = "";

		foreach (var word in words)
		{
			if (word.Length > maxWidth)
			{
				if (current.Length > 0)
				{
					lines.Add(current);
					current = "";
				}

				// Hard-split very long tokens
				for (var i = 0; i < word.Length; i += maxWidth)
					lines.Add(word.Substring(i, Math.Min(maxWidth, word.Length - i)));
				continue;
			}

			var candidate = current.Length == 0 ? word : $"{current} {word}";
			if (candidate.Length <= maxWidth)
			{
				current = candidate;
			}
			else
			{
				lines.Add(current);
				current = word;
			}
		}

		if (current.Length > 0) lines.Add(current);
		return lines.Count > 0 ? lines.ToArray() : ["—"];
	}
}