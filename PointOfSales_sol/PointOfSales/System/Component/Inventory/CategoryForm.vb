Imports System.Data.SQLite

Public Class CategoryForm
    ' 🔹 SQLite connection string
    Private dbName As String = "pos.db"
    Private dbPath As String = Application.StartupPath & "\" & dbName
    Private connectionString As String = "Data Source=" & dbPath & ";Version=3;"

    ' ✅ Event to notify when a new category is added
    Public Event CategoryAdded(categoryName As String)
    ' ✅ Shared event (for POSControl and others)
    Public Shared Event CategoryAddedShared(categoryName As String)

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim categoryName As String = SiticoneTextBox1.Text.Trim()
        Dim description As String = TextBox1.Text.Trim()

        ' Check if fields are empty
        If categoryName = "" Or description = "" Then
            MessageBox.Show("Please fill in both Category and Description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Check description word count
        Dim wordCount As Integer = description.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries).Length
        If wordCount < 10 Then
            MessageBox.Show("Description must have at least 10 words.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Check for duplicate category name
        Using conn As New SQLiteConnection(connectionString)
            Try
                conn.Open()
                Dim checkQuery As String = "SELECT COUNT(*) FROM categories WHERE CategoryName=@catName"
                Using cmd As New SQLiteCommand(checkQuery, conn)
                    cmd.Parameters.AddWithValue("@catName", categoryName)
                    Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    If count > 0 Then
                        MessageBox.Show("Category name already exists. Please enter a different name.", "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using

                ' Insert new category
                Dim insertQuery As String = "INSERT INTO categories (CategoryName, Description) VALUES (@catName, @desc)"
                Using cmd As New SQLiteCommand(insertQuery, conn)
                    cmd.Parameters.AddWithValue("@catName", categoryName)
                    cmd.Parameters.AddWithValue("@desc", description)
                    cmd.ExecuteNonQuery()
                End Using

                ' Notify ProductContent (instance-based)
                RaiseEvent CategoryAdded(categoryName)

                ' Notify POSControl (global/shared)
                RaiseEvent CategoryAddedShared(categoryName)


                MessageBox.Show("Category saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Clear fields after saving
                SiticoneTextBox1.Clear()
                TextBox1.Clear()

            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            Me.Hide()
        End Using
    End Sub

    Private Sub CategoryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Optional: initialization logic
    End Sub


End Class
