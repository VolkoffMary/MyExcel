using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyExcel
{
    public partial class MainForm : Form
    {
        static public Dictionary<string, Cell> dictionary = new Dictionary<string, Cell>(); //усі заповнені комірці будуть зберігатися тут
        private FileIO fileIO = new FileIO();
        public static Dictionary<string, Cell> CellDict
        {
            get { return dictionary; }
        }
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        { 
            createDataGridView();
        }

        private void createDataGridView(int rows = 5, int columns = 5)
        {
            for (int j = 0; j < columns; j++)
                dataGridView.Columns.Add(Name, Name);
            for (int i = 0; i < rows; i++)
                dataGridView.Rows.Add();
        }
        private DialogResult clearTableMsg()
        {
            DialogResult result = MessageBox.Show(
            "Бажаєте зберегти дані таблиці перед продовженням?",
            "Попередження",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning);

            return result;
        }
        private void clearDataGridView(bool clearDict = true)
        {
            if (clearDict)
                dictionary.Clear();

            for (int i = dataGridView.RowCount - 1; i >= 0; i--)
                dataGridView.Rows.RemoveAt(i);
            for (int j = dataGridView.ColumnCount - 1; j >= 0; j--)
                dataGridView.Columns.RemoveAt(j);
        }
        private void fillDataGridView() 
        {
            int rows = dataGridView.RowCount;
            int columns = dataGridView.ColumnCount;
            int row;
            int column;
            foreach (string key in dictionary.Keys)
            {
                Cell.DecomposeName(key, out row, out column);
                Cell cell = dictionary[key];
                if (row >= rows || column >= columns)
                {
                    if (!cell.IsDependantOn)
                        dictionary.Remove(key);
                }
                else
                    dataGridView[column, row].Value = cell.Value;
            }
        }

        private void setRowHeader(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int temp = e.RowIndex;
            dataGridView.Rows[temp].HeaderCell.Value = temp.ToString();
        }
        private void setColumnHeader(object sender, DataGridViewColumnEventArgs e)
        {
            string columnName = Cell.IndexToLetters(dataGridView.ColumnCount - 1);

            e.Column.HeaderText = columnName;
            e.Column.Name = columnName;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) //FormClosingEventArgs
        {
            DialogResult result = MessageBox.Show(
                "Зберегти таблицю перед закриттям програми?",
                "Попередження",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Cancel)
                e.Cancel = true;
            else if (result == DialogResult.Yes)
                fileIO.SaveFile(dataGridView.RowCount, dataGridView.ColumnCount, dictionary);
        }

        private void saveCellExp(string name)
        {
            string exp = textBox.Text;
            Cell cell = new Cell(name, exp);
            if (dictionary.ContainsKey(name))
                dictionary[name].Exp = exp;  
            else
                dictionary.Add(name, cell);
        }
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                int column = dataGridView.CurrentCellAddress.X;
                int row = dataGridView.CurrentCellAddress.Y;
                if (column != -1 && row != -1)
                {
                    string name = Cell.ComposeName(row, column);
                    saveCellExp(name);
                    fillDataGridView();
                }
                else
                    MessageBox.Show("Ви не можете змінювати комірці заголовка таблиці.");
            }
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView.Rows.Add();
        }
        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView.Columns.Add(Name, Name);
        }
        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int row = dataGridView.RowCount - 1;
            if (row >= 1)
            {
                if (rowHasData(row) == true)
                {
                    DialogResult result = MessageBox.Show(
                        "В цьому рядку є дані або в таблиці є клітини, що посилаються на клітини цього рядка. Усе одно видалити рядок?",
                        "Попередження",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                string name;
                int columns = dataGridView.ColumnCount;
                for (int j = 0; j < columns; j++)
                {
                    name = Cell.ComposeName(row, j);
                    if (dictionary.ContainsKey(name))
                    {
                        Cell cell = dictionary[name];
                        if (!cell.IsDependantOn)
                            dictionary.Remove(name);
                        else
                            dictionary[name].Exp = "0";
                    }
                }
                fillDataGridView();
                dataGridView.Rows.RemoveAt(row);
            }

        }
        private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int column = dataGridView.ColumnCount - 1;
            if (column >= 1)
            {
                if (columnHasData(column) == true)
                {
                    DialogResult result = MessageBox.Show(
                        "В цьому стовпчику є дані або в таблиці є клітини, що посилаються на клітини цього стовпця. Усе одно видалити стовпчик?",
                        "Попередження",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                string name;
                int rows = dataGridView.RowCount;
                for (int i = 0; i < rows; i++)
                {
                    name = Cell.ComposeName(i, column);
                    if (dictionary.ContainsKey(name))
                    {
                        Cell cell = dictionary[name];
                        if (!cell.IsDependantOn)
                            dictionary.Remove(name);
                        else
                            dictionary[name].Exp = "0";
                    }
                }
                fillDataGridView();
                dataGridView.Columns.RemoveAt(column);
            }
        }

        private bool rowHasData(int row)
        {
            bool result = false;
            string name;
            int columns = dataGridView.ColumnCount;
            int j = 0;

            while (j < columns && !(result))
            {
                name = Cell.ComposeName(row, j);
                if (dictionary.ContainsKey(name))
                    result = true;
                j++;
            }
            return result;
        }
        private bool columnHasData(int column)
        {
            bool result = false;
            string name;
            int i = 0;
            int rows = dataGridView.RowCount;

            while (i < rows && !(result))
            {
                name = Cell.ComposeName(i, column);
                if (dictionary.ContainsKey(name))
                    result = true;
                i++;
            }
            return result;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileIO.SaveFile(dataGridView.RowCount, dataGridView.ColumnCount, dictionary);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rows = dataGridView.Rows.Count;
            int columns = dataGridView.Columns.Count;
            
            if (rows > 0 && columns > 0)
                switch (clearTableMsg())
                {
                    case DialogResult.Yes:
                        fileIO.SaveFile(dataGridView.RowCount, dataGridView.ColumnCount, dictionary);
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        return;
                    default: break;
                }
            bool wasOpened = fileIO.OpenFile(out rows, out columns, out dictionary);
            if (wasOpened)
            {   
                clearDataGridView(false);
                createDataGridView(rows, columns);
                fillDataGridView();
            }
        }
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.RowCount > 0 && dataGridView.ColumnCount > 0)
                switch (clearTableMsg())
                {
                    case DialogResult.Yes:
                        fileIO.SaveFile(dataGridView.RowCount, dataGridView.ColumnCount, dictionary);
                        clearDataGridView();
                        break;
                    case DialogResult.No:
                        clearDataGridView();
                        break;
                    case DialogResult.Cancel: 
                        return; 
                    default: break;
                }
            createDataGridView();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.RowCount > 0 && dataGridView.ColumnCount > 0)
            {
                switch (clearTableMsg())
                {
                    case DialogResult.Yes:
                        fileIO.SaveFile(dataGridView.RowCount, dataGridView.ColumnCount, dictionary);
                        clearDataGridView();
                        break;
                    case DialogResult.No:
                        clearDataGridView();
                        break;
                    case DialogResult.Cancel:
                        return;
                    default: break;
                }
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
            "В цій програмі Ви можете редагувати таблиці чисельних данних. " +
            "У виразах можуть бути круглі дужки, адреси комірців та цілі числа\n" +
            "Доступні операції:\n"+
            "\t֍Бінарні операції +, -, *, /\n" +
            "\t֍Унарні операції +, -\n" +
            "\t֍Піднесення до степеня ^\n" +
            "\t֍inc, dec\n");
        }

        private void dataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            int column = dataGridView.CurrentCellAddress.X;
            int row = dataGridView.CurrentCellAddress.Y;
            if (column != -1 && row != -1)
            {
                string name = Cell.ComposeName(row, column);
                if (dictionary.ContainsKey(name))
                    textBox.Text = dictionary[name].Exp;
                else
                    textBox.Text = "";
            }
            else
                textBox.Text = "";
        }
    }
}