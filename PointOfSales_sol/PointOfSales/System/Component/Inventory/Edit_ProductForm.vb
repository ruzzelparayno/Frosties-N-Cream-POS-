Imports System.IO
Imports MySql.Data.MySqlClient

Public Class Edit_ProductForm

    Private connectionString As String = "server=localhost;userid=root;password=;database=pos;"
    Public CurrentProductID As String = ""   ' FIXED: ProductID is VARCHAR
    Public CurrentImage As Image = Nothing

    Public Sub LoadProductData(id As String, pname As String, category As String, qty As String, price As String, img As Image)
        CurrentProductID = id

        SiticoneTextBox1.Text = pname
        SiticoneTextBox2.Text = qty
        SiticoneTextBox3.Text = price
        cb_cate.Text = category

        If img IsNot Nothing Then
            CurrentImage = New Bitmap(img)   ' FIX: avoid lock
            PictureBox1.Image = CurrentImage
        Else
            PictureBox1.Image = Nothing
        End If
    End Sub

    Private Sub SiticoneButton2_Click(sender As Object, e As EventArgs) Handles SiticoneButton2.Click
        OpenFileDialog1.Filter = "Image Files|*.jpg;*.png;*.jpeg;*.bmp"

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            ' FIX: Load image WITHOUT locking the file
            Using tempImg As Image = Image.FromFile(OpenFileDialog1.FileName)
                CurrentImage = New Bitmap(tempImg)
            End Using

            PictureBox1.Image = CurrentImage
        End If
    End Sub

    Private Function ConvertImageToBytes(img As Image) As Byte()
        If img Is Nothing Then Return Nothing

        Try
            Using ms As New MemoryStream()
                Dim cloneImg As New Bitmap(img)  ' FIX: clone image before saving
                cloneImg.Save(ms, Imaging.ImageFormat.Png)
                Return ms.ToArray()
            End Using

        Catch ex As Exception
            MessageBox.Show("Image conversion error: " & ex.Message)
            Return Nothing
        End Try
    End Function

    Private Async Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click

        If String.IsNullOrEmpty(CurrentProductID) Then
            MessageBox.Show("Error: No product selected.")
            Exit Sub
        End If

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String = "
                UPDATE products 
                SET ProductName=@name,
                    StockQuantity=@stock,
                    Price=@price,
                    CategoryName=@category,
                    ProductImage=@image
                WHERE ProductID=@id
                "

                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@name", SiticoneTextBox1.Text)
                cmd.Parameters.AddWithValue("@stock", SiticoneTextBox2.Text)
                cmd.Parameters.AddWithValue("@price", SiticoneTextBox3.Text)
                cmd.Parameters.AddWithValue("@category", cb_cate.Text)
                cmd.Parameters.AddWithValue("@id", CurrentProductID)

                ' FIX: safe image conversion
                Dim imgBytes As Byte() = ConvertImageToBytes(CurrentImage)
                cmd.Parameters.AddWithValue("@image", imgBytes)

                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Product updated successfully!")

            If ProductContent.Instance IsNot Nothing Then
                ProductContent.Instance.LoadInventory()
                ProductContent.Instance.SiticoneOverlay1.Show = True
            End If

            Await Task.Delay(1500)

            If ProductContent.Instance IsNot Nothing Then
                ProductContent.Instance.SiticoneOverlay1.Show = False
            End If

            Me.Close()

        Catch ex As Exception
            MessageBox.Show("Error updating product: " & ex.Message)

            If ProductContent.Instance IsNot Nothing Then
                ProductContent.Instance.SiticoneOverlay1.Show = False
            End If
        End Try
    End Sub

End Class
