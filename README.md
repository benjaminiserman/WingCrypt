# WingCrypt
![Downloads](https://img.shields.io/github/downloads/winggar/WingCrypt/total?style=for-the-badge)

WingCrypt is a minimalist WPF app that that allows Windows and Linux users to encrypt and decrypt files and folders with a password.

## Prerequisites

Before you begin, ensure you have met the following requirements:
- [You use a machine supported by .NET 6](https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md)
- You have .NET 6 installed
- You have downloaded the file "build.zip" from the latest release

OR

- [You use a **Windows** machine supported by .NET 6](https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md)
- You **do not** need to have .NET 6 installed
- You have downloaded the file "standalone.zip" from the latest release

OR

- [You use a **Linux** machine supported by .NET 6](https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md)
- You **do not** need to have .NET 6 installed
- You have downloaded the file "linux_standalone.zip" from the latest release
- 
Due to low demand, standalone builds for Mac OSX are not provided. If you'd like a standalone build for Mac OSX or Linux, [contact me](mailto:winggar1228@gmail.com).

## Usage

Windows:
1. Download either "build.zip" or "standalone.zip" from the latest release, depending on your prerequisites.
2. Unzip the file.
3. Find the file "WingCryptWPF.exe" within the unzipped folder and run it.
4. Select files or folders to encrypt/decrypt.
5. Enter a password.
6. Click either the Encrypt or the Decrypt button.

Linux:
1. Download "linux_standalone.zip" from the latest release.
2. Unzip the file.
3. Depending on your architecture, open either the x64 or the arm folder.
4. Copy the three files within into your "/usr/bin" directory.
5. Use the "wingcrypt --help" and proceed from there.

Example: "wingcrypt -p text.txt" will encrypt the file text.txt

Example: "wingcrypt -p text.wenc" will decrypt the file text.wenc

## Contribution
To contribute to WingCrypt, follow these steps:

1. Fork this repository.
2. Create a branch: `git checkout -b <branch_name>`.
3. Make your changes and commit them: `git commit -m '<commit_message>'`
4. Push to the original branch: `git push origin <project_name>/<location>`
5. Create the pull request.

Alternatively see the GitHub documentation on [creating a pull request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request).

## License

![License](https://img.shields.io/github/license/winggar/WingCrypt?style=for-the-badge)
