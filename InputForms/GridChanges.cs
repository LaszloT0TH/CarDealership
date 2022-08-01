using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputForms
{
    internal static class GridChanges
    {
        static DataGridView dataGridView;

        public static DisplayGrid DisplayGridObj { get; set; }

        public static List<int> UserSelectedIndexList;

        public static int dataGridViewRowsCount = 0;

        private static void Start()
        {
            dataGridView = null;

            dataGridViewRowsCount = 0;

            UserSelectedIndexList = null;

            UserSelectedIndexList = new List<int>();

            dataGridView = DisplayGridObj.dataGridView;

            dataGridView.AllowUserToAddRows = false; // the option to add rows is closed

            dataGridViewRowsCount = dataGridView.Rows.Count;
        }

        public static async void Delete()
        {
            Start();
            await DeleteGrid();
        }

        private static Task DeleteGrid()
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (DataGridViewRow item in dataGridView.Invoke(() => dataGridView.SelectedRows))
                {
                    dataGridView.Invoke(() => dataGridView.Rows.RemoveAt(item.Index));
                    int index = dataGridView.Invoke(() => dataGridView.CurrentCell.RowIndex);
                    UserSelectedIndexList.Add(index);
                }
            });
        }

        public static async void Reset()
        {
            Start();
            await ResetGrid();
        }

        private static Task ResetGrid()
        {
            return Task.Factory.StartNew(() =>
            {
                int numRows = dataGridView.Invoke(() => dataGridView.Rows.Count);
                for (int i = 0; i < numRows; i++)
                {
                    int max = dataGridView.Invoke(() => dataGridView.Rows.Count - 1);
                    dataGridView.Invoke(() => dataGridView.Rows.Remove(dataGridView.Rows[max]));
                }
            });
        }
    }
}
