$conn = New-Object System.Data.SqlClient.SqlConnection('Data Source=.;Initial Catalog=KHODIENMAY;Integrated Security=True')
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PhieuNhapKho'"
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Host $reader.GetString(0)
}
$conn.Close()
