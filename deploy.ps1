$StackName = "player2-accounts"

rm -r C:\Projects\vanlune\vanlune-account\Accounts.Application\bin\Release\netcoreapp3.1\publish

dotnet publish C:\Projects\vanlune\vanlune-account -c release

Get-Job | Wait-Job

$7zipPath = "$env:ProgramFiles\7-Zip\7z.exe"
if (-not (Test-Path -Path $7zipPath -PathType Leaf)) {
    throw "7 zip file '$7zipPath' not found"
}
Set-Alias 7zip $7zipPath
$Source = "C:\Projects\vanlune\vanlune-account\Accounts.Application\bin\Release\netcoreapp3.1\publish\*"
$Target = "C:\Projects\vanlune\vanlune-account\Accounts.Application\bin\Release\netcoreapp3.1\publish\Accounts.zip"

7zip a -mx=9 $Target $Source

Get-Job | Wait-Job

aws s3 cp C:/Projects/vanlune/vanlune-account/Accounts.Application/template-accounts.yaml s3://vanlune-bin-dev
aws s3 cp C:\Projects\vanlune\vanlune-account\Accounts.Application\bin\Release\netcoreapp3.1\publish\Accounts.zip s3://vanlune-bin-dev

Get-Job | Wait-Job

$exists = aws cloudformation describe-stacks --stack-name $StackName
if ($exists)
{
	aws cloudformation  update-stack --stack-name $StackName --template-url https://vanlune-bin-dev.s3.amazonaws.com/template-accounts.yaml --capabilities CAPABILITY_IAM CAPABILITY_AUTO_EXPAND
}
else
{
	aws cloudformation create-stack  --stack-name $StackName --template-url https://vanlune-bin-dev.s3.amazonaws.com/template-accounts.yaml --capabilities CAPABILITY_IAM CAPABILITY_AUTO_EXPAND
}
aws cloudformation wait stack-update-complete --stack-name $StackName

aws lambda update-function-code --function-name player2-accounts-create                 --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-authenticate           --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-claim-create           --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-confirm-email          --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-delete                 --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-get-by-filters         --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-recover-password       --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-recover-password-email --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-role-create            --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-role-get-all           --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-role-patch             --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-update                 --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-update-password        --s3-bucket vanlune-bin-dev --s3-key Accounts.zip
aws lambda update-function-code --function-name player2-accounts-user-patch             --s3-bucket vanlune-bin-dev --s3-key Accounts.zip