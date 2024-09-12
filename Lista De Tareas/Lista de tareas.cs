using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace TodoListApp
{
    public partial class TodoForm : Form
    {
        // Cadena de conexión a la base de datos Oracle
        private string connectionString = "User Id=your_user;Password=your_password;Data Source=your_data_source";

        public TodoForm()
        {
            InitializeComponent();
            LoadTasks(); // Cargar las tareas al iniciar la aplicación
        }

        // Método para cargar tareas desde la base de datos y mostrarlas en el DataGridView
        private void LoadTasks()
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Tasks ORDER BY CreatedAt DESC"; // Obtener todas las tareas
                OracleDataAdapter adapter = new OracleDataAdapter(query, connection);
                DataTable tasks = new DataTable();
                adapter.Fill(tasks);
                dataGridViewTasks.DataSource = tasks; // Mostrar las tareas en el DataGridView
            }
        }

        // Método para agregar una tarea
        private void AddTask(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                MessageBox.Show("La descripción no puede estar vacía.");
                return;
            }

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Tasks (Description, CreatedAt, Status) VALUES (:description, :createdAt, 'Pending')";
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":description", description);
                    command.Parameters.Add(":createdAt", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
            LoadTasks(); // Recargar las tareas después de agregar
        }

        // Método para editar una tarea
        private void EditTask(int taskId, string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                MessageBox.Show("La descripción no puede estar vacía.");
                return;
            }

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Tasks SET Description = :description WHERE ID = :taskId";
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":description", description);
                    command.Parameters.Add(":taskId", taskId);
                    command.ExecuteNonQuery();
                }
            }
            LoadTasks(); // Recargar las tareas después de editar
        }

        // Método para eliminar una tarea
        private void DeleteTask(int taskId)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Tasks WHERE ID = :taskId";
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":taskId", taskId);
                    command.ExecuteNonQuery();
                }
            }
            LoadTasks(); // Recargar las tareas después de eliminar
        }

        // Método para marcar una tarea como completada
        private void MarkTaskAsCompleted(int taskId)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Tasks SET Status = 'Completed' WHERE ID = :taskId";
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":taskId", taskId);
                    command.ExecuteNonQuery();
                }
            }
            LoadTasks(); // Recargar las tareas después de actualizar el estado
        }

        // Evento para el botón de agregar tarea
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string description = txtDescription.Text;
            AddTask(description);
        }

        // Evento para el botón de editar tarea
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                int taskId = Convert.ToInt32(dataGridViewTasks.SelectedRows[0].Cells["ID"].Value);
                string description = txtDescription.Text;
                EditTask(taskId, description);
            }
        }

        // Evento para el botón de eliminar tarea
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                int taskId = Convert.ToInt32(dataGridViewTasks.SelectedRows[0].Cells["ID"].Value);
                DeleteTask(taskId);
            }
        }

        // Evento para el botón de marcar como completada
        private void btnComplete_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                int taskId = Convert.ToInt32(dataGridViewTasks.SelectedRows[0].Cells["ID"].Value);
                MarkTaskAsCompleted(taskId);
            }
        }
    }
}
