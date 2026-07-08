## Xkb Layout Creator
This is a simple UI application that allows you to create and install your own keyboard layout. It also can generate .xkb and .xml snippets if you want to add the layout manually.

> [DEPENDENCIES]
> Development: `dotnet-sdk-10.0`
> Release: `dotnet-runtime-10.0`

> [BUILD]
> This project can be built and launched using older versions of dotnet, down to dotnet8.
> After building for proper behavior move the `Elevated/` directory to `bin/Debug/net*` since it expects to have this folder beside the executable.

### How to use
1. Click on the key you want to change and copy-paste/type the desired character in the opened dialog window. U#### and U+#### notation is supported.
2. To not lose your layout use `File->Save`. App will create a .xkblc file, which you can load later via `File->Load`.
3. Use `Manage->Install` to automatically add current layout to the system.

> [!CAUTION]
> 
> While installing the layout, App will automatically backup the original files into `Backups/<filename>.bak`. However, manual backup is recommended. Just in case.
> 
> Files to backup: \
> `.../xkb/symbols/<lang-name>` \
> `.../xkb/rules/evdev.lst` \
> `.../xkb/rules/evdev.xml`

> [!NOTE]
> Alternatively, if you don't trust the App to touch your system files, use `Manage->Export` option to generate .xkb and .xml snippets for you to copy-paste and proceed with manual installation below.

4. To delete a layout use `Manage->Delete`. The App can only delete its own layouts.

5. Logout/Reboot. Your layout should be accessible through DE's Settings or using `setxkbmap`.

### Manual Installation
> [!CAUTION]
> The following files require root privileges to edit (sudo nano/vim). \
> <b>! Backup your files before continuing !</b>

1. In `xkb/symbols/` directory (usually `/usr/share/X11/xkb/symbols`), find the language, of which your layout is a variant of, and paste the .xkb snippet at the bottom. If your layout is not a variation of an existing language, use whichever you feel fits the best.

#### Example (.xkb)
> ```
> partial alphanumeric_keys
> xkb_symbols "ibm238l" {
>     ...
> };
> 
> partial alphanumeric_keys
> xkb_symbols "intl" {
>     ...
> };
> 
> <YOUR SNIPPET HERE>
> ```
2. In `xkb/rules/evdev.lst` add your variant's abbreviation and full name inside the `! variant` section.
#### Example (.lst)
> ```
> ! variant
>   shvn            us: Shavian    ;added this
>   chr             us: Cherokee
>   haw             us: Hawaiian
>   ...
> ```

3. In `xkb/rules/evdev.xml` find `<layoutList>`, then `<layout>` of your language, then `<variantList>` and paste the .xml snippet in-between other variants.

#### Example (.xml)
> ```
> <layout>
>   <configItem>
>     <name>us</name>
>     <shortDescription>en</shortDescription>
>     <description>English (US)</description>
>     <languageList>
>       <iso639Id>eng</iso639Id>
>     </languageList>
>   </configItem>
>   <variantList>
>     <variant>
>       ...
>     </variant>
>     <variant>
>       ...
>     </variant>
>     <YOUR SNIPPET HERE>
>     <variant>
>       ...
>     </variant>
> ```
4. Logout/Reboot. Your layout should be accessible through DE's Settings or using `setxkbmap`.
