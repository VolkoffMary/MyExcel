using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyExcel
{
    public class Cell : DataGridViewTextBoxCell
    {
        const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string address;
        private decimal val; //значення виразу користувача
        private string exp; //вираз користувача
        private List<string> nextCells = new List<string>(); //комірці, вказані в exp
        private List<string> prevCells = new List<string>(); //комірці, які вказують на цей комірець

        public class RecurrenceException : Exception
        {
            public RecurrenceException()
            {
            }
            public RecurrenceException(string message)
                : base(message)
            {
            }
            public RecurrenceException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
        public class OutOfTableException : Exception
        {
            public OutOfTableException()
            {
            }
            public OutOfTableException(string message)
                : base(message)
            {
            }
            public OutOfTableException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public Cell(string adr)
        {
            exp = "0";
            val = 0;
            address = adr;
        }
        public Cell(string adr, string expression)
        {
            address = adr;
            trySetExp(expression);
        }

        new public string Value
        {
            get { return val.ToString(); }
            set { trySetExp(value); }
        }
        public decimal Val
        {
            get { return val; }
        }
        public string Exp
        {
            get { return exp; }
            set { trySetExp(value); }
        }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public bool IsDependantOn
        { 
            get { return prevCells.Count != 0; }
        }

        public void trySetExp(string expression)
        {
            try
            {
                getDependencies(expression);
                RecurrenceCheck(this, this);
                val = Calculator.Evaluate(expression);
                exp = expression;
                reEvaluateCellValues(address);
            }
            catch (RecurrenceException e)
            {
                string errorMessage = e.Message;
                DialogResult result = MessageBox.Show(
                "Вираз створює рекурсивну залежність між значеннями комірців.\n",
                "Попередження",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            }
            catch (ArgumentException e)
            {
                string errorMessage = e.Message;
                DialogResult result = MessageBox.Show(
                "У виразі є синтаксична помилка. Повідомлення помилки:\n" +
                errorMessage,
                "Попередження",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            }
            catch (Exception e)
            {
                string errorMessage = e.Message;
                DialogResult result = MessageBox.Show(
                "Повідомлення помилки:\n" +
                errorMessage,
                "Попередження",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            }
        }

        //встановлення залежностей
        public void getDependencies(string expression)
        {
            clearOldDependencies();
            Calculator.GetDepCells(expression, address, out nextCells);
            fillNewDependencies();
        }
        public void clearOldDependencies()
        {
            var dict = MainForm.CellDict;
            foreach (string key in nextCells)
                if (dict.ContainsKey(key) && dict[key].prevCells.Contains(address))
                    dict[key].prevCells.Remove(address);
        }
        public void fillNewDependencies()
        {
            var dict = MainForm.CellDict;
            foreach (string key in nextCells)
            {
                if (!dict.ContainsKey(key))
                {
                    Cell cell = new Cell(key);
                    dict.Add(key, cell);
                }
                if (!dict[key].prevCells.Contains(address))
                    dict[key].prevCells.Add(address);
            }
        }

        //перевірка на наявність рекурсії
        public void RecurrenceCheck(Cell Current, Cell Initial)
        {
            if (Current.nextCells.Contains(Initial.address))
                throw new RecurrenceException("Виникла рекурсія"); //true;
            var dict = MainForm.CellDict;
            foreach (string cellName in Current.nextCells)
            {
                if (!(dict.ContainsKey(cellName)))
                {
                    Cell cell = new Cell(cellName);
                    dict.Add(cellName, cell);
                }
                RecurrenceCheck(MainForm.CellDict[cellName], Initial);
            }
        }

        //перераховуємо потрібні комірці
        private void reEvaluateCellValues(string name)
        {
            var dictionary = MainForm.CellDict;
            foreach (string cellName in prevCells)
            {
                Cell cell = dictionary[cellName];
                cell.val = Calculator.Evaluate(cell.exp);
            }
        }
        public static string ComposeName(int row, int column)
        {
            return IndexToLetters(column) + row.ToString();
        }
        public static void DecomposeName(string name, out int row, out int column)
        {
            int separationIndex=0;
            while (ALPHABET.Contains(name[separationIndex]))
                separationIndex++;

            string columnStr = name.Substring(0, separationIndex);
            string rowStr = name.Substring(separationIndex);
            int.TryParse(rowStr, out row);
            double baseDouble;
            column = 0;
            while (columnStr.Length > 1)
            {
                baseDouble = Math.Pow(ALPHABET.Length, columnStr.Length - 1);
                column += (int)baseDouble * (ALPHABET.IndexOf(columnStr[0]) + 1);
                columnStr.Substring(1, columnStr.Length - 1);
            }
            column += ALPHABET.IndexOf(columnStr[0]);
        }
        //переводить індекс стовпця в буквенний вигляд
        public static string IndexToLetters(int i)
        {
            string result = null;
            int units = i % ALPHABET.Length;
            int tens = i / ALPHABET.Length;

            result += ALPHABET[units];
            while (tens != 0)
            {
                units = tens % (ALPHABET.Length + 1);
                tens = (tens - units) / ALPHABET.Length;
                result = ALPHABET[units - 1] + result;
            }
            return result;
        }
    }
}