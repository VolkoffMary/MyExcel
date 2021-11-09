using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MyExcel
{
    class FileIO
    {
        private SaveFileDialog saveFileDialog = new SaveFileDialog();
        private OpenFileDialog openFileDialog = new OpenFileDialog();

        public FileIO()
        {
            saveFileDialog.DefaultExt = "TABLE";
            saveFileDialog.AddExtension = true;
            saveFileDialog.ValidateNames = true;
            openFileDialog.Filter = "Table files(*.TABLE)|*.TABLE|All files(*.*)|*.*";
            saveFileDialog.Filter = "Table files(*.TABLE)|*.TABLE|All files(*.*)|*.*";
        }
        public void SaveFile(int rows, int columns, Dictionary<string, Cell> dict)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog.FileName;
            StreamWriter streamWriter = new StreamWriter(filename);
            try
            {
                string temp = rows.ToString() + "#" + columns.ToString();
                streamWriter.WriteLine(temp);
                var keys = dict.Keys;
                foreach (string key in keys)
                {
                    int row;
                    int column;
                    Cell.DecomposeName(key, out row, out column);
                    Cell cell = dict[key];
                    if ((row < rows && column < columns) || cell.IsDependantOn)
                    {
                        temp = key + "#" + cell.Val + "#" + cell.Exp;
                        streamWriter.WriteLine(temp);
                    }
                    else 
                        dict.Remove(key);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Сталася помилка при записі файлу. Спробуйте ще раз.");
            }
            finally
            {
                streamWriter.Close();
            }
        }

        // открытие файла
        public bool OpenFile(out int rows, out int columns, out Dictionary<string, Cell> dict)
        {
            rows = 0;
            columns = 0;
            dict = new Dictionary<string, Cell>();
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return false;
            string filename = openFileDialog.FileName;
            StreamReader streamReader = new StreamReader(filename);
            try 
            {
                string line = streamReader.ReadLine();
                char[] delim = new char[] { '#' };
                string[] list = line.Split(delim, StringSplitOptions.None);
                rows = int.Parse(list[0]);
                columns = int.Parse(list[1]);

                line = streamReader.ReadLine();
                while (line != null)
                {
                    list = line.Split(delim, StringSplitOptions.None);
                    string key = list[0];
                    string exp = list[2];
                    Cell cell = new Cell(key, exp);
                    dict.Add(key, cell);
                    line = streamReader.ReadLine();
                }
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Сталася помилка при зчитуванні файлу. Спробуйте ще раз.");
                return false;
            }
            finally
            {
                streamReader.Close();
            }
        }
    }
}
