$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$bin = Join-Path $root "bin"
$output = Join-Path $bin "DroneFactory.exe"
$program = Join-Path $root "Program.cs"

if (-not (Test-Path $bin)) {
    New-Item -ItemType Directory -Force -Path $bin | Out-Null
}

$csc64 = Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$csc32 = Join-Path $env:WINDIR "Microsoft.NET\Framework\v4.0.30319\csc.exe"

if (Test-Path $csc64) {
    $csc = $csc64
} elseif (Test-Path $csc32) {
    $csc = $csc32
} else {
    throw "C# compiler not found. Install a .NET SDK or .NET Framework developer tools."
}

& $csc /nologo /target:exe "/out:$output" "$program"

if ($LASTEXITCODE -ne 0) {
    throw "Build failed."
}

Write-Host "Build OK: $output"
