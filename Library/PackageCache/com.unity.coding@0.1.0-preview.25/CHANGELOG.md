# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.1.0-preview.25] - 2023-05-30

### Changed

- Upgraded uncrustify binaries (lin64, mac64 and win32).

## [0.1.0-preview.24] - 2023-02-10

### Fixed

- Added support for the updated and backported AssetDatabase calls in Unity 2023.1.

## [0.1.0-preview.23] - 2023-02-03

### Added

- Added support for OSX Arm64.
- Added support for the updated AssetDatabase calls in Unity 2023.2.

## [0.1.0-preview.22] - 2022-07-28

### Changed

- Removed dependency on nuget.moq package

## [0.1.0-preview.21] - 2022-05-31

### Changed

- Updated the api scraper

## [0.1.0-preview.20] - 2020-05-06

### Changed

- Republishing the package because of an incorrect upload of version 0.1.0-preview.19

## [0.1.0-preview.19] - 2020-04-22

### Changed

- Fixed a bug causing incorrect asset path resolve. This prevented some files to be validated during format validation. Issue existed only on Windows.
- Implemented a workaround for an issue that hanged Unity when a new script was added on MacOS (#24)

## [0.1.0-preview.18] - 2020-04-16

### Changed

- Documentation update
- Added API for code format validation
- Turned off console logging for the formatter
- Fixed api scraper to include local packages when invoked manually

## [0.1.0-preview.17] - 2020-03-26

### Changed

- Disabled an uncrustify setting because of a bug https://github.com/uncrustify/uncrustify/issues/2705

## [0.1.0-preview.16] - 2020-03-20

### Changed

- Fixed scraping API with docs code

## [0.1.0-preview.15] - 2020-03-12

### Changed

- Fixed compilation error

## [0.1.0-preview.14] - 2020-03-12

### Changed

- Removed restrictions on API scraping to allow on-disk packages to be scraped if they are in mutable folders.
- Updated ApiScraper.exe
- Fixed coding-cli.exe tests
- Disabled code formatting in batch mode and test runs

### Added

- Added new Formatting.Format API to allow scraping from script.
- Added new Formatting.ValidateFilesFormatted API to validate file formatting.
- Added new public API for scraping api with docs

## [0.1.0-preview.13] - 2019-11-21

- Code restructure
- Docs update

## [0.1.0-preview.12] - 2019-11-19

- Reverted Moq to be a package dependancy due to conflict with other packages

## [0.1.0-preview.11] - 2019-11-18

- Updated access modifiers and documentations

### Changed

- Added an extra check to make sure the the scraper is not invoked during compilation
- Introduced api for scraping api manually
- Fixed issued with manual api validation

## [0.1.0-preview.10] - 2019-11-05

### Changed

- Added an extra check to make sure the the scraper is not invoked during compilation
- Introduced api for scraping api manually
- Fixed issued with manual api validation

## [0.1.0-preview.9] - 2019-09-30

### Changed

- Fixed incorrect return value for api validation

## [0.1.0-preview.8] - 2019-09-30

### Changed

- Removed api validation when running in batch mode
- Introduced api for invoking validation manually

## [0.1.0-preview.7] - 2019-09-19

### Changed

- Disabled scraping for non-editor builds
- Made validation insensitive to line endings

## [0.1.0-preview.6] - 2019-09-11

### Changed

- Renamed extension of API Scrapings to .api
- Implemented pre import callback to avoid double compilation
- Fixed error that cancelled formating when progress dialog was shown
- Moved editorconfig and shoudly dependencies to be in-project dlls until they are available in production repository

## [0.1.0-preview.5] - 2019-04-01

### Changed

- Added API Scraping
- Doc updates to README.
- Fixed the .editorconfig template to not generate errors in Rider, plus added more rules.
- Added support for 2019.1, though .2 is required to format local packages outside the project root.

## [0.1.0-preview.4] - 2019-03-21

### Changed

- Avoid unnecessary formatting after a import triggered by a previous formatting.
- Set execution permission for uncrustify in mac64/linux64 folders.
- Select uncrustify executable based on current platform.
- Fix crash when backup files were made read-only.
- Fix incompatibility with *UnityEditor.PS4.Extensions.dll* (compilation errors due to NiceIO name clashes).
- Allow users to disable auto formating (disable_auto_format option in .editorconfig).

## [0.1.0] - 2019-02-25

### This is the first release of *Unity Package \<Coding\>*
