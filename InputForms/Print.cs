using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputForms
{
    internal static class Print
    {
        static DataGridView dataGridView;

        static PrintPreviewDialog printPreviewDialog;

        static PrintDocument printDocument;

        static Bitmap bitmap;

        public static DisplayGrid DisplayGridObj { get; set; }
        
        public static void Printing()
        {
            getDataGridViewDetails();
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }

        private static void getDataGridViewDetails()
        {
            InitializePrintPreviewDialog();
            dataGridView = DisplayGridObj.dataGridView;
        }

        private static void InitializePrintPreviewDialog()
        {
            printDocument = new PrintDocument();
            printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.AutoScrollMargin = new Size(0, 0);
            printPreviewDialog.AutoScrollMinSize = new Size(0, 0);
            printPreviewDialog.ClientSize = new Size(400, 300);
            printPreviewDialog.Location = new Point(29, 29);
            printDocument.PrintPage += new PrintPageEventHandler(document_PrintPage);
            printPreviewDialog.Enabled = true;
            printPreviewDialog.UseAntiAlias = true;
        }

        private static void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            int height = dataGridView.Height;
            dataGridView.Height = dataGridView.RowCount * dataGridView.RowTemplate.Height * 2;

            bitmap = new Bitmap(dataGridView.Width, dataGridView.Height);
            dataGridView.DrawToBitmap(bitmap, new Rectangle(0, 0, dataGridView.Width, dataGridView.Height));

            printPreviewDialog.PrintPreviewControl.Zoom = 1;
            dataGridView.Height = height;
            e.Graphics.DrawImage(bitmap, new Rectangle(0, 0, dataGridView.Width, dataGridView.Height));
        }
    }
}
