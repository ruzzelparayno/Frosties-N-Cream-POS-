Imports System.Data.SQLite
Imports System.Security.Cryptography
Imports System.Text

Public Class UserContent
    ' 🔹 SQLite connection string (points to local file)
    Private dbName As String = "pos.db"
    Private dbPath As String = Application.StartupPath & "\" & dbName
    Private conn As New SQLiteConnection("Data Source=" & dbPath & ";Version=3;")

    Public Shared Instance As UserContent

    Private Sub UserContent_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' 🔹 Load users when the form/control opens
        Instance = Me
        LoadUsers()
        cb_sq.DropDownStyle = ComboBoxStyle.DropDownList
        cb_ur.DropDownStyle = ComboBoxStyle.DropDownList
        SiticoneTextBox6.UseSystemPasswordChar = True
        SiticoneTextBox7.UseSystemPasswordChar = True
        SiticoneTextBox3.UseSystemPasswordChar = True
        SiticoneTextBox6.PasswordChar = "•"c
        SiticoneTextBox7.PasswordChar = "•"c
        SiticoneTextBox3.PasswordChar = "•"c
        PictureBox2.Show()
        PictureBox4.Show()
        PictureBox6.Show()
        PictureBox1.Hide()
        PictureBox3.Hide()
        PictureBox5.Hide()
    End Sub

    ' 🔹 Function to hash a string with SHA256
    Private Function HashText(input As String) As String
        Using sha As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(input)
            Dim hash As Byte() = sha.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToLower()
        End Using
    End Function

    Public Sub LoadUsers()
        Try
            Using conn As New SQLiteConnection("Data Source=" & dbPath & ";Version=3;")
                conn.Open()

                Dim cmd As New SQLiteCommand("SELECT userid, username, role, Secret_Question, email FROM users", conn)
                Dim da As New SQLiteDataAdapter(cmd)
                Dim dt As New DataTable
                da.Fill(dt)
                Guna2DataGridView1.DataSource = dt

                If Guna2DataGridView1.Columns.Contains("userid") Then
                    Guna2DataGridView1.Columns("userid").AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
                End If
                If Guna2DataGridView1.Columns.Contains("username") Then
                    Guna2DataGridView1.Columns("username").AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                End If
                If Guna2DataGridView1.Columns.Contains("role") Then
                    Guna2DataGridView1.Columns("role").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        End Try
    End Sub

    Private Sub ClearFields()
        SiticoneTextBox5.Clear() 'USERNAME
        SiticoneTextBox6.Clear() 'PASSWORD
        SiticoneTextBox6.Enabled = False
        SiticoneTextBox3.Enabled = False 'CONFIRM PASSWORD
        SiticoneTextBox1.Enabled = False 'EMAIL
        SiticoneTextBox3.Clear()
        SiticoneTextBox7.Clear() 'SECRET ANSWER
        SiticoneTextBox1.Clear()
        cb_ur.SelectedIndex = -1
        cb_sq.SelectedIndex = -1
    End Sub

    Private Async Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        If SiticoneTextBox5.Text = "" Or SiticoneTextBox6.Text = "" Or SiticoneTextBox3.Text = "" Or cb_ur.Text = "" Or cb_sq.Text = "" Or SiticoneTextBox1.Text = "" Or SiticoneTextBox7.Text = "" Then
            MessageBox.Show("Please fill all required fields.")
            Exit Sub
        End If

        If SiticoneTextBox6.Text <> SiticoneTextBox3.Text Then
            MessageBox.Show("Passwords do not match.")
            Exit Sub
        End If

        Try
            Using conn As New SQLiteConnection("Data Source=" & dbPath & ";Version=3;")
                conn.Open()

                SiticoneOverlay1.Show = True
                Await Task.Delay(1500)

                ' 🔹 Check duplicate username
                Dim checkUserQuery = "SELECT COUNT(*) FROM users WHERE username=@username"
                Using checkCmd As New SQLiteCommand(checkUserQuery, conn)
                    checkCmd.Parameters.AddWithValue("@username", SiticoneTextBox5.Text)
                    Dim count = Convert.ToInt32(checkCmd.ExecuteScalar())
                    If count > 0 Then
                        MessageBox.Show("Username is already used. Please choose another one.", "Duplicate Username", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End Using

                ' 🔹 Check duplicate email
                Dim checkEmailQuery = "SELECT COUNT(*) FROM users WHERE email=@Email"
                Using checkCmd As New SQLiteCommand(checkEmailQuery, conn)
                    checkCmd.Parameters.AddWithValue("@Email", SiticoneTextBox1.Text)
                    Dim count = Convert.ToInt32(checkCmd.ExecuteScalar())
                    If count > 0 Then
                        MessageBox.Show("Email is already used. Please use another email.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End Using

                ' 🔹 Insert new user
                Dim query = "INSERT INTO users(username, password, role, Secret_Question, Secret_Answer, email) VALUES(@username, @password, @role, @secretQ, @secretA, @Email)"
                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", SiticoneTextBox5.Text)
                    cmd.Parameters.AddWithValue("@password", HashText(SiticoneTextBox6.Text))
                    cmd.Parameters.AddWithValue("@role", cb_ur.Text)
                    cmd.Parameters.AddWithValue("@secretQ", cb_sq.Text)
                    cmd.Parameters.AddWithValue("@secretA", HashText(SiticoneTextBox7.Text))
                    cmd.Parameters.AddWithValue("@Email", SiticoneTextBox1.Text)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            LoadUsers()
            SiticoneOverlay1.Show = False

            MessageBox.Show("User added successfully!",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)

            ClearFields()

        Catch ex As Exception
            MessageBox.Show("Error inserting user: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SiticoneButton1_Click(sender As Object, e As EventArgs)
        SiticoneTextBox5.Clear()
        SiticoneTextBox6.Clear()
        SiticoneTextBox6.Enabled = True
        SiticoneTextBox3.Enabled = True
        SiticoneTextBox3.Clear()
        SiticoneTextBox7.Clear()
        SiticoneTextBox1.Clear()
        cb_ur.SelectedIndex = -1
        cb_sq.SelectedIndex = -1
        SiticoneTextBox5.Focus()
        SiticoneTextBox1.Enabled = True
    End Sub

    Private Sub Guna2DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub

        Dim row = Guna2DataGridView1.Rows(e.RowIndex)

        EditUser.CurrentUserID = CInt(row.Cells("userid").Value)
        EditUser.txtUsername.Text = row.Cells("username").Value.ToString()
        EditUser.cbRole.Text = row.Cells("role").Value.ToString()
        EditUser.txtEmail.Text = row.Cells("email").Value.ToString()

        EditUser.Show()
    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        SiticoneTextBox6.UseSystemPasswordChar = True
        SiticoneTextBox6.PasswordChar = "•"c

        PictureBox1.Hide()
        PictureBox2.Show()
    End Sub

    ' Click PictureBox2 → show PictureBox1, show password
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        SiticoneTextBox6.UseSystemPasswordChar = False

        PictureBox2.Hide()
        PictureBox1.Show()
    End Sub

    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        SiticoneTextBox3.UseSystemPasswordChar = True
        SiticoneTextBox3.PasswordChar = "•"c

        PictureBox3.Hide()
        PictureBox4.Show()
    End Sub

    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click
        SiticoneTextBox3.UseSystemPasswordChar = False

        PictureBox3.Show()
        PictureBox4.Hide()
    End Sub

    Private Sub PictureBox5_Click(sender As Object, e As EventArgs) Handles PictureBox5.Click
        SiticoneTextBox7.UseSystemPasswordChar = True
        SiticoneTextBox7.PasswordChar = "•"c

        PictureBox5.Hide()
        PictureBox6.Show()
    End Sub

    Private Sub PictureBox6_Click(sender As Object, e As EventArgs) Handles PictureBox6.Click
        SiticoneTextBox7.UseSystemPasswordChar = False

        PictureBox5.Show()
        PictureBox6.Hide()
    End Sub

    Private Sub SiticoneTextBox5_KeyDown(sender As Object, e As KeyEventArgs) Handles SiticoneTextBox7.KeyDown, SiticoneTextBox6.KeyDown, SiticoneTextBox5.KeyDown, SiticoneTextBox3.KeyDown, SiticoneTextBox1.KeyDown, cb_ur.KeyDown, cb_sq.KeyDown
        If e.KeyCode = Keys.Enter Then
            BtnSave.PerformClick()
        End If
    End Sub
End Class
