# Run .Net Framework tests
$testDlls = ls -r *.artifacts\*\Debug\*.Tests.dll | ? FullName -match "\\net\d+\\" `
  | % FullName | sort-object -Unique
nunit3-console $testDlls --result=".artifacts\nunit-netfw-results.xml"

# Run .Net Core tests
$testDlls = ls -r *.artifacts\*\Debug\*.Tests.dll | ? FullName -notmatch "\\net\d+\\" `
  | % FullName | sort-object -Unique
dotnet vstest $testDlls  --logger "trx;LogFileName=..\.artifacts\nunit-netcore-results.xml"

# Upload files
$wc = New-Object 'System.Net.WebClient'
$testResults = ls *.artifacts\nunit-*.xml | % FullName | sort-object -Unique
foreach ($testResult in $testResults) {
  echo "UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $testResult"
  $wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit/$($env:APPVEYOR_JOB_ID)", $testResult)  
}