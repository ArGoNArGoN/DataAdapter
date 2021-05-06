using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UseSqlDataAdapter
{
	public partial class Form1 
		: Form
	{
		public String ConnectionString { get; set; } = "Строку я не выложу в гит";
		public NpgsqlConnection cn { get; set; }
		public DataTable Table { get; set; }

		public Boolean IsEdit { get; set; }

		public Form1()
		{
			InitializeComponent();
		}


		/// ТУТ СТАРТ 

		private void Form1_Load(object sender, EventArgs e)
		{
			var strSql = "SELECT * FROM \"Teams\"";
			
			try
			{
				cn = new NpgsqlConnection(ConnectionString);

				/// берем датаАдаптер и подключение
				var dataAdapter = new NpgsqlDataAdapter(strSql, cn);
				var table = new DataTable(); /// Создаем таблицу с данными

				/// заполняем таблицу данными из БД через DataAdapter
				dataAdapter.Fill(table); 
				dataAdapter.Update(table); /// хз зачем, это не нужно

				LoadDataGrid(table); /// скармливаем таблицу DataGrid
				LoadList(table); /// скармливаем таблицу listBox


				/// запоминаем это объект (он находится чуть-чуть выше (20 строчка))
				Table = table;
				/// ставим у таблицы Pr. Key 
				Table.PrimaryKey = new DataColumn[1] { table.Columns[0] }; /// да-да, только массивом 
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void LoadList(DataTable table)
		{
			/// скармливаем таблицу
			listTeams.DataSource = table;

			/// Это не важно, но десь я установил отображение 
			/// по свойству title и значению id (SelectedValue, теперь будет возвращать id)
			listTeams.DisplayMember = "title";
			listTeams.ValueMember = "id";
		}
		
		public void LoadDataGrid(DataTable table)
		{
			/// скармливаем таблицу
			teams.DataSource = table;

			/// Это не важно
			teams.RowHeadersVisible = false;
			teams.Columns[0].Visible = false;
			teams.Columns[1].Width = 150;
			teams.Columns[2].Width = 80;
			teams.Columns[3].Width = 100;
			teams.Columns[4].Visible = false;
			teams.Columns[5].Visible = false;
		}

		/// гинада смотреть
		private void button1_Click(object sender, EventArgs e)
		{
			button3.Enabled = true;

			button3.Text = "Добавить";
			button3.Click += new System.EventHandler(this.button3_Click_1);
			button3.Click -= new System.EventHandler(this.button3_Click);
			IsEdit = false;
		}

		/// нинада смотреть x2
		private void button2_Click(object sender, EventArgs e)
		{
			button3.Enabled = true;

			button3.Text = "Сохранить изменения";
			button3.Click += new System.EventHandler(this.button3_Click);
			button3.Click -= new System.EventHandler(this.button3_Click_1);
			IsEdit = true;
		}

		private void button3_Click(object sender, EventArgs e)
        {
			/// берем значения с формы и приводим к String
			var title = this.title.Text; 
			var raiting = checkBox1.Checked ? "NULL" : this.raiting.Value.ToString(); 
			var percentWin = checkBox2.Checked ? "NULL" : this.percentWin.Value.ToString();

			/// создаем запросик
			var sql = $"UPDATE \"Teams\" SET \"Title\" = '{title}', \"Raiting\" = {raiting}, \"PercentWin\" = {percentWin} WHERE \"Id\"={teams.SelectedRows[0].Cells[0].Value}";
            
			try
            {
				cn.Open();
				var com = new NpgsqlCommand(sql, cn);

				/// я закоментил, ибо один раз случайно снес все данные из таблицы и.. блин, хочу сдохнуть, как же мне тогда было стыдно, что я не посмотрел в запрос и не увидел, что не прописал условие изменения строки 
				/* if (com.ExecuteNonQuery() != 1)
					throw new Exception("ну там запрос не выполнился, я не знаю почему, но мне как-то по..."); */

				/// ищем строку по Pr. key (мы задали в 55 строчке) SelectedRows[0].Cells[0] - там хранится id
				var row = Table.Rows.Find(teams.SelectedRows[0].Cells[0].Value);

				/// Ищем столбец и заполняем его
				row["Title"] = title;

				/// Ищем столбец и заполняем его
				if (checkBox1.Checked)
					row["Raiting"] = DBNull.Value;
				else
					row["Raiting"] = this.raiting.Value;

				/// Ищем столбец и заполняем его
				if (checkBox1.Checked)
					row["PercentWin"] = DBNull.Value;
				else
					row["PercentWin"] = this.percentWin.Value;
			

				/// и всо
			}
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message);
            }
            finally
            {
				cn.Close();
            }
		}

        private void button3_Click_1(object sender, EventArgs e)
        {
			var title = this.title.Text;
			var raiting = checkBox1.Checked ? "NULL" : this.raiting.Value.ToString();
			var percentWin = checkBox2.Checked ? "NULL" : this.percentWin.Value.ToString();

			/// создаем запросик
			var sql = $"INSERT INTO \"Teams\"(\"Id\", \"Title\", \"Raiting\", \"PercentWin\") VALUES (1001, '{title}', {raiting}, {percentWin})";
			
			try
			{
				cn.Open();
				var com = new NpgsqlCommand(sql, cn);

				/* if (com.ExecuteNonQuery() != 1)
					throw new Exception("ну там запрос не выполнился, я не знаю почему, но мне как-то по..."); */

				/// создаем строку
				var row = Table.NewRow();

				row["Id"] = 789; /// Заглушка у меня проблема с бд и я не могу вставить значения, поэтому костыль

				/// Ищем столбец и заполняем его
				row["Title"] = title;

				/// Ищем столбец и заполняем его
				if (checkBox1.Checked)
					row["Raiting"] = DBNull.Value;
				else
					row["Raiting"] = this.raiting.Value;

				/// Ищем столбец и заполняем его
				if (checkBox1.Checked)
					row["PercentWin"] = DBNull.Value;
				else
					row["PercentWin"] = this.percentWin.Value;

				/// Добавляем строку в таблицу
				Table.Rows.Add(row);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				cn.Close();
			}
		}

		/// это нинада
        private void teams_CellClick(object sender, DataGridViewCellEventArgs e)
        {
			teams.ClearSelection();
			teams.Rows[e.RowIndex].Selected = true;

            if (IsEdit)
            {
				percentWin.Value = 0;
				raiting.Value = 0;

				var s = teams.Rows[e.RowIndex];
				title.Text = s.Cells[1].Value.ToString();


				if (s.Cells[2].Value.ToString() == "")
					checkBox1.Checked = true;
				else
					raiting.Value = (Decimal)s.Cells[2].Value;


				if (s.Cells[3].Value.ToString() == "")
					checkBox2.Checked = true;
				else
					percentWin.Value = (Decimal)s.Cells[3].Value;
			}
        }
    }
}
