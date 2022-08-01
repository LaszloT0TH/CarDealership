using Car_Dealership;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace InputForms
{
    internal class DisplayGrid : InputField
    {
        public DataGridView dataGridView;

        public DisplayGrid(string text, string[] options, Control parent = null) : base(text, parent)
        {
            Items(options);
            (input as DataGridView).TabIndex = 1;
            Print.DisplayGridObj = this;
            GridChanges.DisplayGridObj = this;
        }

        public DisplayGrid AddGridRows(object[] rows)
        {
            foreach (string[] rowArray in rows)
            {
                dataGridView.Rows.Add(rowArray);
            }
            return this;
        }
        
        private void Items(string[] options)
        {
            for (int i = 1; i < options.Length + 1; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();

                dataGridView.Columns.AddRange(new DataGridViewColumn[] { column });

                column.HeaderText = options[i - 1];
                column.Name = "column" + i.ToString();
                column.MinimumWidth = 6;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.Resizable = DataGridViewTriState.True;
                column.ReadOnly = true;
            }
        }

        protected override Control CreateField()
        {
            dataGridView = new DataGridView();
            dataGridView.Size = new Size(1126, 643);
            dataGridView.Location = new Point(10, 10);
            dataGridView.Margin = new Padding(4, 4, 4, 4);
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersWidth = 51;
            dataGridView.ScrollBars = ScrollBars.Both;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            return dataGridView;
        }
    }
}
