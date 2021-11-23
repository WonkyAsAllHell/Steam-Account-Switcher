![Preview](Resources/preview.png)

# Steam Account Switcher
Steam Account Switcher is a tool that will let you easily switch between steam accounts. It handles closing steam, signing into the new account and reopening steam all at a click of a button. This tool is perfect for anyone that has multiple steam accounts or has multiple people that share the same computer.

*Note for this tool to work, all steam accounts will need to have been signed in at least once on the PC.*

When this tool is run it first reads 'loginusers.vdf' and grabs the info of the accounts that have previously logged in *Note: no password info or 2fa tokens are stored in 'loginusers.vdf'*. Then the GUI displays the accounts with their current profile picture for the user to select. After the user selects a steam account all steam accounts listed in 'loginusers.vdf' will have the MostRecent field set to 0 except for the selected account. The selected account will also have the timestamp updated. Then the steam registry keys are updated to match the info added to 'loginusers.vdf'

example 'loginusers.vdf' file
```
"77777777777777777" // steam id
{
	"AccountName"		"account name"
	"PersonaName"		"public name"
	"RememberPassword"		"1"
	"MostRecent"		"1"
	"WantsOffline"		"0"
	"TimeStamp"		"1637653053"
}
```

To get started simply:
1. Download the latest zip from the releases panel
2. Make sure you have logged into each steam account that you want to switch between at least once
3. run the SteamAccountSwitcher.exe
4. select the account you want to switch to (the currently signed in account will be highlighted in green)
