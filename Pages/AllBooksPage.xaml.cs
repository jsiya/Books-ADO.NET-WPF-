using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Books_ADO.NET_.Pages
{
    /// <summary>
    /// qeyd: insert ancaq ilk acilanda ve refresh olanda etmek olur filtrledikden sonra insert deaktiv edilib 
    /// </summary>
    public partial class AllBooksPage : Page
    {
        SqlConnection? connection = null;
        DataTable? dataTable = null;

        public AllBooksPage()
        {
            InitializeComponent();
            DataContext = this;
            connection = new SqlConnection();
            connection.ConnectionString = "Data Source=DESKTOP-UCO0K2D;Initial Catalog=Library;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            
        }

        public void SelectBooksFromDB(string query)
        {
            try
            {
                connection?.Open();
               
                using SqlCommand? sqlCommand = new(query, connection);
                using SqlDataReader reader = sqlCommand.ExecuteReader();

                bool isColumnName = true;

                dataTable = new();
                while (reader.Read())
                {
                    if (isColumnName)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dataTable?.Columns.Add(reader.GetName(i));
                        }
                    }
                    isColumnName = false;

                    DataRow row = dataTable.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader[i];
                    }
                    dataTable.Rows.Add(row);
                }
                BooksDataGrid.DataContext = null;
                BooksDataGrid.DataContext = dataTable;
                BooksDataGrid.ItemsSource = dataTable.DefaultView;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection?.Close();
            }
        }

        public void SelectCategoriesFromDB()
        {
            try
            {
                connection?.Open();
                string query = "SELECT * FROM Categories";
                using SqlCommand? sqlCommand = new(query, connection);
                using SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    CategoryComboBox.Items.Add($"{reader[1]}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection?.Close();
            }
        }

        public void SelectAuthorsFromDB()
        {
            try
            {
                connection?.Open();
                string query = "SELECT * FROM Authors";
                using SqlCommand? sqlCommand = new(query, connection);
                using SqlDataReader reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    AuthorsComboBox.Items.Add($"{reader[1]} {reader[2]}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection?.Close();
            }
        }

        public void InsertData()
        {
            try
            {
                connection?.Open();

                if (BooksDataGrid.SelectedItem != null)
                {
                    DataRowView selectedRow = (DataRowView)BooksDataGrid.SelectedItem;

                    int id = Convert.ToInt32(selectedRow["Id"]);
                    string newBookName = selectedRow["Name"].ToString();
                    string newBookComment = selectedRow["Comment"].ToString();
                    int pages = Convert.ToInt32(selectedRow["Pages"]);
                    int yearPress = Convert.ToInt32(selectedRow["YearPress"]);
                    int quantity = Convert.ToInt32(selectedRow["Quantity"]);
                    int categoryId = Convert.ToInt32(selectedRow["Id_Category"]); 
                    int themeId = Convert.ToInt32(selectedRow["Id_Themes"]); 
                    int pressId = Convert.ToInt32(selectedRow["Id_Press"]); 
                    int authorId = Convert.ToInt32(selectedRow["Id_Author"]); 

                    string insertQuery = "INSERT INTO Books (Id, Name, Comment, Pages, YearPress, Quantity, Id_Category, Id_Themes, Id_Press, Id_Author) " +
                                         "VALUES (@Id, @Name, @Comment, @Pages, @YearPress, @Quantity, @CategoryId, @ThemeId, @PressId, @AuthorId)";

                    using SqlCommand sqlCommand = new(insertQuery, connection);
                    sqlCommand.Parameters.AddWithValue("@Name", newBookName);
                    sqlCommand.Parameters.AddWithValue("@Comment", newBookComment);
                    sqlCommand.Parameters.AddWithValue("@Pages", pages);
                    sqlCommand.Parameters.AddWithValue("@Id", id);
                    sqlCommand.Parameters.AddWithValue("@YearPress", yearPress);
                    sqlCommand.Parameters.AddWithValue("@Quantity", quantity);
                    sqlCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                    sqlCommand.Parameters.AddWithValue("@ThemeId", themeId);
                    sqlCommand.Parameters.AddWithValue("@PressId", pressId);
                    sqlCommand.Parameters.AddWithValue("@AuthorId", authorId);

                    sqlCommand.ExecuteNonQuery();

                }
                else
                {
                    MessageBox.Show("select a row to insert.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection?.Close();
            }
            string refreshQuery = "SELECT * FROM Books";
            SelectBooksFromDB(refreshQuery);
        }


        public void UpdateData()
        {
            try
            {
                connection?.Open();

                if (BooksDataGrid.SelectedItem != null)
                {
                    DataRowView selectedRow = (DataRowView)BooksDataGrid.SelectedItem;

                    int bookId = Convert.ToInt32(selectedRow["Id"]);

                    string updateQuery = "UPDATE Books SET Name = @Name, Comment = @Comment, Pages = @Pages, YearPress = @YearPress, Quantity = @Quantity " +
                                         "WHERE Id = @BookId"; 

                    using SqlCommand sqlCommand = new(updateQuery, connection);
                    sqlCommand.Parameters.AddWithValue("@Name", selectedRow["Name"]);
                    sqlCommand.Parameters.AddWithValue("@Comment", selectedRow["Comment"]);
                    sqlCommand.Parameters.AddWithValue("@Pages", selectedRow["Pages"]);
                    sqlCommand.Parameters.AddWithValue("@YearPress", selectedRow["YearPress"]);
                    sqlCommand.Parameters.AddWithValue("@Quantity", selectedRow["Quantity"]);
                    sqlCommand.Parameters.AddWithValue("@BookId", bookId);

                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("select a row to update.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection?.Close();
            }
            string refreshQuery = "SELECT * FROM Books";
            SelectBooksFromDB(refreshQuery);
        }

        public void DeleteData()
        {
            try
            {
                connection?.Open();

                if (BooksDataGrid.SelectedItem != null)
                {
                    DataRowView selectedRow = (DataRowView)BooksDataGrid.SelectedItem;

                    int bookId = Convert.ToInt32(selectedRow["Id"]);

                    string deleteQuery = "DELETE FROM Books WHERE Id = @BookId";

                    using SqlCommand sqlCommand = new(deleteQuery, connection);
                    sqlCommand.Parameters.AddWithValue("@BookId", bookId);

                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("select a row to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection?.Close();
            }

            string refreshQuery = "SELECT * FROM Books";
            SelectBooksFromDB(refreshQuery);
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            BooksDataGrid.CanUserAddRows = false;
            if(SearchBar.Text.Length > 0)
            {
                AuthorsComboBox.SelectedItem = null;
                CategoryComboBox.SelectedItem = null;
                string query = $"SELECT Books.Id, Books.Name, Books.Comment, Books.Pages, Books.YearPress, Books.Quantity, Categories.Name AS [Category Name], Authors.FirstName, Authors.LastName FROM Books JOIN Categories ON Categories.Id = Books.Id_Category JOIN Authors ON Books.Id_Author = Authors.Id WHERE Books.Name = '{SearchBar.Text}'";
                SelectBooksFromDB(query);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BooksDataGrid.CanUserAddRows = true;
            SelectCategoriesFromDB();
            SelectAuthorsFromDB();
            string query = "SELECT * FROM Books";
            SelectBooksFromDB(query);
        }

        private void AuthorsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(AuthorsComboBox.SelectedItem != null)
            {
                BooksDataGrid.CanUserAddRows = false;
                string query = $"SELECT Books.Id, Books.Name, Books.Comment, Books.Pages, Books.YearPress, Books.Quantity, Categories.Name AS [Category Name], " +
                   $"Authors.FirstName, Authors.LastName FROM Books JOIN Categories ON Categories.Id = Books.Id_Category JOIN Authors ON Books.Id_Author = Authors.Id " +
                   $"WHERE Books.Id_Author = {AuthorsComboBox.SelectedIndex + 1} AND Books.Id_Category = {CategoryComboBox.SelectedIndex + 1}";
                SelectBooksFromDB(query);
                if (dataTable.Rows.Count == 0)
                    MessageBox.Show("There is no such books");
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CategoryComboBox.SelectedItem != null)
            {
                BooksDataGrid.CanUserAddRows = false;
                AuthorsComboBox.IsEnabled = true;
                if (AuthorsComboBox.SelectedItem != null) AuthorsComboBox.SelectedItem = null;
                string query = $"SELECT Books.Id, Books.Name, Books.Comment, Books.Pages, Books.YearPress, Books.Quantity, Categories.Name AS [Category Name], " +
                   $"Authors.FirstName, Authors.LastName FROM Books JOIN Categories ON Categories.Id = Books.Id_Category JOIN Authors ON Books.Id_Author = Authors.Id " +
                   $"WHERE Books.Id_Category = {CategoryComboBox.SelectedIndex + 1}";
                SelectBooksFromDB(query);
                if (dataTable.Rows.Count == 0)
                    MessageBox.Show("There is no such books");
            }

        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

            SearchButton.IsEnabled = true;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            BooksDataGrid.CanUserAddRows = true;
            AuthorsComboBox.SelectedItem = null;
            AuthorsComboBox.IsEnabled= false;
            CategoryComboBox.SelectedItem = null;
            string query = "SELECT * FROM Books";
            SelectBooksFromDB(query);
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            InsertData();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteData();
        }
    }
}
