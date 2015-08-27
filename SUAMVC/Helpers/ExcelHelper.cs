using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SUADATOS;
using SUAMVC.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.DynamicData;
using System.IO;

namespace SUAMVC.Helpers
{
    public class ExcelHelper
    {

        private suaEntities db = new suaEntities();
        private SpreadsheetDocument spreadSheetDocument;
        private WorkbookPart workBookPart;
        private WorksheetPart workSheetPart;
        private Worksheet workSheet;
        private Workbook workBook;
        private FileVersion fileVersion;

        public ExcelHelper()
        {
        }

        public ExcelHelper(String fileName) {
            spreadSheetDocument = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook);
            workBookPart = spreadSheetDocument.AddWorkbookPart();

            workSheetPart = workBookPart.AddNewPart<WorksheetPart>();
            workBook = new Workbook();
            fileVersion = new FileVersion();
            fileVersion.ApplicationName = "Microsoft Office Excel";
            
        
        }

        public Sheet createSheet(String sheetName, SheetData sheetData, UInt32Value id)
        {

            workSheet = new Worksheet();
            WorkbookStylesPart workBookStylesPart = this.workBookPart.AddNewPart<WorkbookStylesPart>();
            workBookStylesPart.Stylesheet = CreateStylesheet();
            workBookStylesPart.Stylesheet.Save();

            workSheet.Append(sheetData);
            workSheetPart.Worksheet = workSheet;
            workSheetPart.Worksheet.Save();

            Sheet sheet = new Sheet();
            sheet.Name = sheetName;
            sheet.SheetId = id;
            sheet.Id = this.workBookPart.GetIdOfPart(workSheetPart);

            return sheet;
        }

        public void saveWorkBook(Sheets sheets){
                
                this.workBook.Append(sheets);
                workBook.Append(fileVersion);
                this.spreadSheetDocument.WorkbookPart.Workbook = this.workBook;
                this.spreadSheetDocument.WorkbookPart.Workbook.Save();
                this.spreadSheetDocument.Close();
        }


        public void makeExcel(String excelName)
        {
            try
            {
                //Creamos el objeto del workbook
                SpreadsheetDocument xl = SpreadsheetDocument.Create(excelName, SpreadsheetDocumentType.Workbook);

                WorkbookPart wbp = xl.AddWorkbookPart();
                WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
                Workbook wb = new Workbook();
                FileVersion fv = new FileVersion();
                fv.ApplicationName = "Microsoft Office Excel";

                Worksheet ws = new Worksheet();
                WorkbookStylesPart wbsp = wbp.AddNewPart<WorkbookStylesPart>();
                // add styles to sheet
                wbsp.Stylesheet = CreateStylesheet();
                wbsp.Stylesheet.Save();

                List<DetallePago> detallePago = db.DetallePagoes.ToList();

                SheetData sd = CreateContentRow(); 
                ws.Append(sd);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();
                
                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Sheet1";
                sheet.SheetId = 1;
                sheet.Id = wbp.GetIdOfPart(wsp);

                sheets.Append(sheet);
                wb.Append(fv);
                wb.Append(sheets);

                xl.WorkbookPart.Workbook = wb;
                xl.WorkbookPart.Workbook.Save();
                xl.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }

        }

        string[] headerColumns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP" };
        public SheetData CreateContentRow()
        {

            SheetData sheetData = new SheetData();
            int index = 2;
            List<Empleado> empleados = new List<Empleado>();

            empleados = (from s in db.Empleados
                         orderby s.id
                         select s).ToList();
            //Create the new row.
            
            //Create the cells that contain the data.
            foreach (Empleado emp in empleados)
            {
                int i = 0;
                Row r = new Row();
                r.RowIndex = (UInt32)index;
                Cell firstCell = CreateTextCell(headerColumns[i], emp.nombre, index);
                r.AppendChild(firstCell);

                firstCell = CreateTextCell(headerColumns[i + 1], emp.apellidoMaterno, index);
                r.AppendChild(firstCell);

                firstCell = CreateTextCell(headerColumns[i+2], emp.calleNumero, index);
                r.AppendChild(firstCell);

                firstCell = CreateTextCell(headerColumns[i + 3], emp.nss, index);
                r.AppendChild(firstCell);

                sheetData.AppendChild(r);
                index++;
            }
            return sheetData;
        }


        

        public Cell CreateTextCell(string header, string text, int index)
        {
            //Create a new inline string cell.
            Cell c = new Cell();
            c.DataType = CellValues.InlineString;
            c.CellReference = header + index;
            //Add text to the text cell.
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = text;
            inlineString.AppendChild(t);
            c.AppendChild(inlineString);
            return c;
        }

        public SheetData CreateSheetData()
        {
            SheetData sheetData1 = new SheetData();
            Row row1 = new DocumentFormat.OpenXml.Spreadsheet.Row() { 
                RowIndex = (UInt32Value)1U, Spans = new ListValue<StringValue>() { 
                    InnerText = "1:3" 
                }, DyDescent = 0.25D };
            Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellReference = "A1", StyleIndex = (UInt32Value)1U };

            row1.Append(cell1);

            Row row2 = new DocumentFormat.OpenXml.Spreadsheet.Row() { RowIndex = (UInt32Value)2U, Spans = new ListValue<StringValue>() { InnerText = "1:3" }, DyDescent = 0.25D };
            Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellReference = "B2", StyleIndex = (UInt32Value)2U };

            row2.Append(cell2);

            DocumentFormat.OpenXml.Spreadsheet.Row row3 = new DocumentFormat.OpenXml.Spreadsheet.Row() { RowIndex = (UInt32Value)3U, Spans = new ListValue<StringValue>() { InnerText = "1:3" }, DyDescent = 0.25D };
            DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellReference = "C3", StyleIndex = (UInt32Value)3U };

            row3.Append(cell3);

            sheetData1.Append(row1);
            sheetData1.Append(row2);
            sheetData1.Append(row3);

            return sheetData1;
        }

        public Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            //Creamos las fuentes a usar
            Fonts fonts1 = new Fonts() { 
                Count = (UInt32Value)1U, KnownFonts = true 
            };

            //Creamos la primer fuente Bold negra 11px
            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Arial" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };
            Bold bFont = new Bold();

            //Agregamos los estilos definidos arriba a la fuente
            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);
            font1.Append(bFont);

            //Agregamos la fuente a nuestra cola de fuentes
            fonts1.Append(font1);

            //Creamos una segunda fuente sin ser Bold, negra 8px
            Font font2 = new Font();
            FontSize fontSize2 = new FontSize() { Val = 8D };
            Color color2 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName2 = new FontName() { Val = "Arial" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

            //Agregamos los estilos definidos arriba a la fuente
            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontScheme2);

            //Agregamos la fuente a nuestra cola de fuentes
            fonts1.Append(font2);

            //Creamos la tercer fuente Bold negra 8px
            Font font3 = new Font();
            FontSize fontSize3 = new FontSize() { Val = 8D };
            Color color3 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName3 = new FontName() { Val = "Arial" };
            FontFamilyNumbering fontFamilyNumbering3 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme3 = new FontScheme() { Val = FontSchemeValues.Minor };
            Bold bFont3 = new Bold();

            //Agregamos los estilos definidos arriba a la fuente
            font3.Append(fontSize3);
            font3.Append(color3);
            font3.Append(fontName3);
            font3.Append(fontFamilyNumbering3);
            font3.Append(fontScheme3);
            font3.Append(bFont3);

            fonts1.Append(font3);

            //Creamos nuestra cola de rellenos en este caso serán 5
            Fills fills1 = new Fills() { Count = (UInt32Value)5U };

            // FillId = 0 - Sin relleno
            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };
            fill1.Append(patternFill1);

            // FillId = 1 - Relleno gris
            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColorGray = new ForegroundColor() { Rgb = "FFC0C0C0" };
            BackgroundColor backgroundColorGray = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill2.Append(foregroundColorGray);
            patternFill2.Append(backgroundColorGray);
            fill2.Append(patternFill2);

            // FillId = 2, Gris
            Fill fill3 = new Fill();
            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFC0C0C0" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);
            fill3.Append(patternFill3);

            // FillId = 3, Azul
            Fill fill4 = new Fill();
            PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor() { Rgb = "FF0070C0" };
            BackgroundColor backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);
            fill4.Append(patternFill4);

            // FillId = 4, Amarillo
            Fill fill5 = new Fill();
            PatternFill patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor3 = new ForegroundColor() { Rgb = "FFFFFF00" };
            BackgroundColor backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill5.Append(foregroundColor3);
            patternFill5.Append(backgroundColor3);
            fill5.Append(patternFill5);

            //Agregan los rellenos a la cola de relleno
            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);
            fills1.Append(fill5);

            //Se genera la cola de bordes/marcos a usar
            Borders borders1 = new Borders() { Count = (UInt32Value)2U };

            //Creamos el primer borde sin color
            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            borders1.Append(border1);

            //Creamos el segundo borde con color negro
            Border border2 = new Border();
            LeftBorder leftBorder2 = new LeftBorder();
            Color blackColor = new Color() { Indexed = (UInt32Value)22U };
            leftBorder1.Style = BorderStyleValues.Thick;

            leftBorder2.Append(blackColor);

            RightBorder rightBorder2 = new RightBorder();
            blackColor = new Color() { Indexed = (UInt32Value)22U };
            rightBorder2.Style = BorderStyleValues.Thick;
            rightBorder2.Append(blackColor);


            TopBorder topBorder2 = new TopBorder();
            blackColor = new Color() { Indexed = (UInt32Value)22U };
            topBorder2.Append(blackColor);
            topBorder2.Style = BorderStyleValues.Thick;

            BottomBorder bottomBorder2 = new BottomBorder();
            blackColor = new Color() { Indexed = (UInt32Value)22U };
            bottomBorder2.Append(blackColor);
            bottomBorder2.Style = BorderStyleValues.Thick;
            
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            //Agregamos nuestras definiciones al borde2
            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            //Agregamos nuestro border2 a la cola de borders
            borders1.Append(border2);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)2U };
            CellFormat cellStyleFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };
            CellFormat cellStyleFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)1U, FontId = (UInt32Value)1U, FillId = (UInt32Value)1U, BorderId = (UInt32Value)1U };

            cellStyleFormats1.Append(cellStyleFormat1);
            cellStyleFormats1.Append(cellStyleFormat2);

            //Creamos nuestra pila de formatos
            CellFormats pilaFormatosDeCelda = new CellFormats() { Count = (UInt32Value)6U };
            
            //Creamos nuestros formatos
            // *** Rellenos ***
            //0 - No Relleno
            //1 - Relleno Gris
            //2 - Relleno Rojo
            //3 - Relleno Azul
            //4 - Relleno Amarillo
            // *** Bordes ***
            //0 - No Bordes
            //1 - Bordes Negros

            // 0 - Fuente Bold, Sin relleno, sin Bordes
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            //1 - Fuente Normal, Sin relleno, sin Bordes
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            //2 - Fuente Normal, Sin relleno, sin Bordes, Alineados al centro
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Center } };
            //3 - Fuente Normal, Sin Relleno, Con Bordes 
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Center } };
            //4 - Fuente Bold, Relleno Gris, con Bordes, Alineado al centro
            //CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)1U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFill = true, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Center } };
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Center } };
            //5 - Fuente Bold, Relleno Azul, con Bordes, Alineado al centro
            CellFormat cellFormat6 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true, Alignment = new Alignment{ Horizontal = HorizontalAlignmentValues.Center}};

            //Agregamos nuestros formatos a la pila definida
            pilaFormatosDeCelda.Append(cellFormat1);
            pilaFormatosDeCelda.Append(cellFormat2);
            pilaFormatosDeCelda.Append(cellFormat3);
            pilaFormatosDeCelda.Append(cellFormat4);
            pilaFormatosDeCelda.Append(cellFormat5);
            pilaFormatosDeCelda.Append(cellFormat6);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
             //X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            // stylesheetExtension1.Append(slicerStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(pilaFormatosDeCelda);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);

            return stylesheet1;
        }

        /**
         * Creamos una nueva celda
         * Parametros: 
         *    - content: Contenido de la celda
         *    - reference: Referencia de la celda
         *    - styleIndex: estilo utilizado en numero
         *    - type: tipo Celda
         */
        public Cell createNewCell(Row row, String content, String reference, UInt32Value styleIndex, CellValues type)
        {
            Cell cell = new Cell();
            cell = new Cell() { CellValue = new CellValue(content), CellReference = reference, StyleIndex = (UInt32Value)styleIndex, DataType = type };

            return cell;
        }

        /**
         * Agregamos una nueva celda a la fila
         * Parametros: 
         *    - index: indice de posición
         *    - row: fila utilizada
         *    - content: Contenido de la celda
         *    - reference: Referencia de la celda
         *    - styleIndex: estilo utilizado en numero
         *    - type: tipo Celda
         */
        public Row addNewCellToRow(int index, Row row, String content, String reference, UInt32Value styleIndex, CellValues type)
        {
            row = new Row();
            row.RowIndex = (UInt32)index;
            Cell cell = new Cell();
            cell = new Cell() { CellValue = new CellValue(content), CellReference = reference, StyleIndex = (UInt32Value)styleIndex, DataType = type };
            row.AppendChild(cell);

            return row;
        }
    }
}