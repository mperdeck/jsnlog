# Must set up cntrl-P cntrl-M as shortcut for opening package manager console!
# Tools | Options | Keyboard

# Show commands containing:
# view.p


# -------------------------

Param(
  [Parameter(Mandatory=$True,Position=1)][string]$testProjectsDir,
  [Parameter(Mandatory=$True,Position=2)][string]$scratchProjectsDir,
  [Parameter(Mandatory=$True,Position=3)][string]$projectName,
  [Parameter(Mandatory=$True,Position=4)][string]$packageName,
  [Parameter(Mandatory=$True,Position=5)][string]$generatedPackagesDir
)

$testProjectDir = [System.IO.Path]::Combine($testProjectsDir, $projectName)
$scratchProjectDir = [System.IO.Path]::Combine($scratchProjectsDir, $projectName)
$scratchProjectFilePath = [System.IO.Path]::Combine($scratchProjectDir, $projectName + ".csproj")

write-output  $testProjectsDir
write-output  $scratchProjectsDir
write-output  $projectName
write-output  $packageName
write-output  $generatedPackagesDir
write-output  $testProjectDir
write-output  $scratchProjectDir
write-output  $scratchProjectFilePath

# ------------------------------------
# Remove scratch project (if it exists), than make a fresh copy from testProjectsDir

Remove-Item $scratchProjectDir -recurse -force
write-output 'after remove'

Copy-Item $testProjectDir $scratchProjectsDir -recurse
write-output 'after copy'

# Remove any read-only flags from the files in the scratch project, so NuGet can update them

$files = (get-childitem $scratchProjectDir -include * -recurse)

foreach($file in $files) 
{ 
    if (($file.Attributes -band [System.IO.FileAttributes]::Directory) -ne [System.IO.FileAttributes]::Directory) 
	{ 
	    Set-ItemProperty -path $file.FullName -Name IsReadOnly -Value $false 
    }
} 

write-output 'remove read-only'

# ------------------------------------

add-type -AssemblyName microsoft.VisualBasic

add-type -AssemblyName System.Windows.Forms

# ------------ Start Visual Studio and activate its window

invoke-item "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe"
start-sleep -Milliseconds 2000
try 
{ 
    [Microsoft.VisualBasic.Interaction]::AppActivate("Start Page - Microsoft Visual Studio (Administrator)") 
} 
catch [System.Exception] 
{
    write-output 'AppActivate - not found'
}

# ------------ Load the project

# List of key codes:
# http://msdn.microsoft.com/en-us/library/office/aa202943(v=office.10).aspx

[System.Windows.Forms.SendKeys]::SendWait("^+o")
[System.Windows.Forms.SendKeys]::SendWait("$scratchProjectFilePath~")
start-sleep -Milliseconds 15000

# ------------ Open package manager console and import nuget package

[System.Windows.Forms.SendKeys]::SendWait("^p^m")
start-sleep -Milliseconds 30
[System.Windows.Forms.SendKeys]::SendWait("Install-Package $packageName -Source $generatedPackagesDir~")
start-sleep -Milliseconds 30000

# ------------ Hit F5 to run the site

[System.Windows.Forms.SendKeys]::SendWait("{F5}")

write-output 'success'



