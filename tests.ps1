# Run .Net Framework tests
$logFilePath = '.artifacts\nunit-netfw-results.xml'
$testDlls = ls -r '.artifacts\*\Debug\*.Tests.dll' | ? FullName -match '\\net\d+\\' `
  | % FullName | Sort-Object -Unique
nunit3-console $testDlls --result=$logFilePath

## replace assemply name in tests
$matchPattern = 'name="(?''name''.*?)\.dll" fullname="(?''fullname''.*?\\(?''fw''net[^\\]*)\\[^\\]*?)\.dll"'
$replacement = 'name="${name} (${fw}).dll" fullname="${fullname} (${fw}).dll"'
(cat $logFilePath) -Replace $matchPattern, $replacement | Out-File -Encoding UTF8 $logFilePath

# Run .Net Core tests
$testDlls = ls -r '.artifacts\*\Debug\*.Tests.dll' | ? FullName -notmatch '\\net\d+\\' `
  | % FullName | sort-object -Unique
dotnet vstest $testDlls --results-directory '.\.artifacts\' --logger 'trx;LogFileName=nunit-netcore-results.xml'

### TODO: replace assembly name in TRX files

# Upload files
$wc = New-Object 'System.Net.WebClient'
$testResults = ls *.artifacts\nunit-*.xml | % FullName | sort-object -Unique
foreach ($testResult in $testResults) {
  echo "UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $testResult"
  $wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit3/$($env:APPVEYOR_JOB_ID)", $testResult)  
}