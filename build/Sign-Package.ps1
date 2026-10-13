
# Signs the packages with the Uno Platform certificate in Azure Key Vault (uno-codesign-vault variable group),
# like Uno.Wasm.Bootstrap and Uno.XamlBehaviors. Fails instead of skipping: release def 31 pushes uno builds to nuget.org.
if([string]::IsNullOrEmpty($Env:VaultSignClientSecret)){
	Write-Error "VaultSignClientSecret is empty: link and authorize the uno-codesign-vault variable group"
	exit 1
}

dotnet tool install --tool-path . sign --version 0.9.1-beta.25278.1

$nupkgs = gci $Env:ArtifactDirectory\*.nupkg -recurse | Select -ExpandProperty FullName

foreach ($nupkg in $nupkgs){
	Write-Host "Submitting $nupkg for signing"

	.\sign code azure-key-vault `
		$nupkg `
		--publisher-name "$Env:SignPackageName" `
		--description "$Env:SignPackageDescription" `
		--description-url "$Env:build_repository_uri" `
		--azure-key-vault-tenant-id "$Env:VaultSignTenantId" `
		--azure-key-vault-client-id "$Env:VaultSignClientId" `
		--azure-key-vault-client-secret "$Env:VaultSignClientSecret" `
		--azure-key-vault-certificate "$Env:VaultSignCertificate" `
		--azure-key-vault-url "$Env:VaultSignUrl" `
		--verbosity information
	if ($LASTEXITCODE -ne 0) {
		exit 1
	}
	Write-Host "Finished signing $nupkg"
}

Write-Host "Sign-package complete"
