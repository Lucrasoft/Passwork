
 [![nuget badge](https://img.shields.io/nuget/v/Lucrasoft.Passwork.svg)](https://www.nuget.org/packages/Lucrasoft.Passwork/)

 # .NET Connector for Passwork Pro (self hosted)

A .NET Standard 2.1 library for the REST API of Passwork Pro (self hosted) instance. Based on v4 of the REST API.
 

This .NET Connector uses a database-like context object, to access vaults, folders and passwords.
It supports both master password enabled/disabled situations.

## Getting started

Get yourself a context object, which gives access to all options.

``` cs
//create the client 
var client = new Client("https://your-server/api/v4/");

//get the context object by login
var ctx = await client.Login("<apikey>", "<optional masterpassword>");

//Adding a vault, returns a `IVault` object.
var vault = await ctx.AddVault("SampleVault", false);

//With the vault object , you can add Folders (and passwords directly in the vault).
var folder = vault.AddFolder("SampleFolder");

//Creating a password in the folder object you just obtained.
var p = folder.CreatePassword();
p.Name = "Hello";
p.Pass = "Some secretpassword";
await p.Save();

```

To read the password field and the custom fields in an `IPassword` object, you must first unlock the secrets by calling `Unlock`.
Example:

``` cs
var folder = await vault.GetFolderByName("MyFolder");
var passes = await folder.GetPasswords();

//lets use the first password in this folder.
var p = passes[0];
await p.Unlock();

//after Unlock , the password is readable
Console.WriteLine($"{p.Pass}");

```

You can also search passwords, based on some query string, or tags, or colors.
Searching can start from the context, or from a specific vault.

``` cs
var passes = await ctx.Query()
                    .WhereFieldsLike("vpn")
                    .WhereTag("topsecret")
                    .Get();
```



## Context

From the context object you receive, after the login, you can perform the following methods.

Methods (async) | Returns
---|---
`AddVault(string name, bool isPrivate)` | [`IVault`](#ivault)
`GetVaults()` | [`IVault[]`](#ivault) 
`GetVaultByName(string name)` | [`IVault`](#ivault)
`GetVaultByID(string id)` | [`IVault`](#ivault)
`GetPasswordByID(string id)` |  [`IPassword`](#ipassword)
`GetPasswordsRecent()` |  [`IPassword[]`](#ipassword)
`GetPasswordsFavorite()` |  [`IPassword[]`](#ipassword)
`GetTags()` | `string[]`
`GetColors()` | `int[]`

## IVault

Properties are `Id`, `Name` 

Methods (async) | Returns
-|-
`CreatePassword()` | [`IPassword`](#ipassword)
`GetPasswords()` | [`IPassword[]`](#ipassword)
`GetFolders()` | [`IFolder[]`](#ifolder)
`GetFolderByName(string name)` | [`IFolder`](#ifolder)
`AddFolder(string name)` | [`IFolder`](#ifolder)
`Delete()` | `bool`
`GetColors()` | `int[]`
`GetTags()` | `string[]`

## IFolder

Properties are : `Id`, `Name`, `AmountFolders`, `AmountPasswords`.

Methods (async) | Returns 
-|-
`Refresh()` | -
`AddFolder(string name)` | [`IFolder`](#ifolder)
`Rename(string name)` | [`IFolder`](#ifolder)
`GetFolders()` | [`IFolder[]`](#ifolder)  
`CreatePassword()` | [`IPassword`](#ipassword)
`GetPasswords()` | [`IPassword[]`](#ipassword)  
`Delete()` | `bool`  

## IPassword

Properties are : `Id`, `Name`, `Url`, `Password`, `Login`, `Description`, `Tags`, `Color`, `CustomRecords`.


Methods (async) | Returns 
-|-
`Save()` | -
`Unlock()` | -
`SetFavorite(bool IsFavorite)` | -

