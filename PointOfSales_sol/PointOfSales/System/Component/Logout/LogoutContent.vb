Public Class LogoutContent

    ' ================================
    ' YES BUTTON (SiticoneButton1)
    ' ================================

    Private Sub logout_load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim clear As New AccountContent

    End Sub
    Private Sub SiticoneButton1_Click(sender As Object, e As EventArgs) Handles SiticoneButton1.Click

        ' Clear user session data
        ClearUserData()

        ' Clear Login textboxes
        Login.SiticoneTextBox1.Text = ""
        Login.SiticoneTextBox2.Text = ""
        Login.SiticoneTextBox1.Focus()

        ' Show message AFTER logout
        MessageBox.Show("You have been logged out successfully!",
                    "Logout",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)

        ' Show Login form
        Login.Show()
        LoginPass.currentUsername = ""
        LoginPass.currentRole = ""


        ' 🔥 IMPORTANT FIX — tell Dashboard not to show exit message
        Dashboard.IsLoggingOut = True

        ' Close Dashboard ONLY
        Dashboard.Hide()


    End Sub



    ' ================================
    ' CLEAR USER SESSION DATA
    ' ================================
    Private Sub ClearUserData()
        LoginPass.currentUsername = String.Empty

        Try
            For Each ctrl As Control In Me.Controls
                If TypeOf ctrl Is TextBox Then
                    DirectCast(ctrl, TextBox).Clear()
                ElseIf TypeOf ctrl Is Label Then
                    DirectCast(ctrl, Label).Text = ""
                End If
            Next
        Catch ex As Exception
            ' Ignore errors during cleanup
        End Try

    End Sub


    ' ================================
    ' NO BUTTON (SiticoneButton2)
    ' ================================
    Private Sub SiticoneButton2_Click(sender As Object, e As EventArgs) Handles SiticoneButton2.Click
        MessageBox.Show("Logout cancelled.",
                        "Cancelled",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)
    End Sub

End Class
