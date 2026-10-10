$files = Get-ChildItem -Path "D:\Projects for PC\Unity\My project\Assets" -Recurse -Filter *.cs
foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    $content = [regex]::Replace($content, '(?m)^[ \t]*//.*$', '')
    $content = $content.Replace("Kenchi Casino", "Casino Project")
    $content = $content.Replace("Kenchi", "Student")
    Set-Content -Path $f.FullName -Value $content
}
