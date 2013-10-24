Public Class Form1
	Public Class Package
		Public FullPath As String
		Public FilesNum As Integer
		Public PackageSize As Integer
	End Class

	Public Class PackageComparer
		Implements IComparer(Of Package)

		Public Function Compare(ByVal x As Package, _
		ByVal y As Package) As Integer _
		 Implements IComparer(Of Package).Compare
			Dim str1 As String = x.FullPath.Substring(0, x.FullPath.LastIndexOf("\"))
			Dim str2 As String = y.FullPath.Substring(0, y.FullPath.LastIndexOf("\"))
			Dim i As Integer = str1.CompareTo(str2)
			If i = 0 Then
				If x.PackageSize = y.PackageSize Then
					Return 0
				ElseIf x.PackageSize < y.PackageSize Then
					Return 1
				ElseIf x.PackageSize > y.PackageSize Then
					Return -1
				End If
			End If
			Return i
		End Function
	End Class

	Private Function GetSize(ByVal path As String) As Integer
		Dim size As Integer = 0
		Dim files() As String = IO.Directory.GetFiles(path)
		For Each file In files
			size += New IO.FileInfo(file).Length
		Next
		Dim folders() As String = IO.Directory.GetDirectories(path)
		For Each folder In folders
			size += GetSize(folder)
		Next
		Return size
	End Function

	Private Sub FillInPackage(ByRef package As Package, ByVal path As String)
		With package
			.FullPath = path.Substring(0, path.LastIndexOf("\")).Replace(Application.StartupPath + "\", "")
			.FilesNum = IO.Directory.GetFiles(.FullPath).Count + IO.Directory.GetDirectories(.FullPath).Count
			.PackageSize = GetSize(.FullPath)
		End With
	End Sub

	Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Dim dsms() As String = IO.Directory.GetFiles(Application.StartupPath(), "*.dsm", IO.SearchOption.AllDirectories)
		Dim list As New List(Of Package)
		Array.Sort(dsms)
		For Each dsm In dsms
			Dim package As New Package
			FillInPackage(package, dsm)
			list.Add(package)
		Next
		list.Sort(New PackageComparer())

		For Each Package In list
			Dim lvi As New ListViewItem
			lvi.Text = Package.FullPath
			lvi.SubItems.Add(Package.FilesNum.ToString)
			lvi.SubItems.Add((Package.PackageSize \ 1024).ToString + " KB")
			lvi.Tag = Package
			ListView1.Items.Add(lvi)
		Next
		Me.Size = New System.Drawing.Size(Me.Size.Width, Me.Size.Height + 1)
	End Sub

	Private Sub ListView1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.Resize
		ColumnHeader2.Width = 100
		ColumnHeader3.Width = 100
		ColumnHeader1.Width = ListView1.Width - ColumnHeader2.Width - ColumnHeader3.Width - 50
	End Sub
End Class
