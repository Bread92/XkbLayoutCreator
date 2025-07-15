### Linux Keyboard Layout Creator
This is a simple UI application that generates .xkb and .xml snippets for you to manually copy into the corresponding system files. This program does not modify your system files.

#### How to use
1. Click on the key you want to change and copy-paste/type the desired character in the opened dialog window.
2. To not lose your progress (if you need to close the app) use File->Save. The app will create a .klc file, which you can load later via File->Load.
3. After you've done all the changes click File->Export. It will generate .xkb and .xml snippets for you to copy-paste into your system files.
> [!CAUTION]
> The following files require root privileges to edit (sudo nano/vim). <b>! Backup your files before continuing !</b>
4. Go to symbols/ directory (/usr/share/X11/xkb/symbols), find the language, of which your layout is a variant of, and paste the .xkb snippet at the bottom (or in-between other variants).

### Example: English (Shavian)
> ```
> partial alphanumeric_keys
> xkb_symbols "ibm238l" {
>     ...
> };
> 
> <YOUR SNIPPET HERE>
> 
> partial alphanumeric_keys
> xkb_symbols "intl" {
>     ...
> };
>
> <OR HERE>
> ```
5. Go to xkb/rules/ directory and add in evdev.lst inside the `! variant` section your variant's abbreviation and full name.
### Example (Shavian)
> ```
> ! variant
>   chr             us: Cherokee
>   shvn            us: Shavian
>   haw             us: Hawaiian
>   ...
> ```

6. In xkb/rules/ directory in evdev.xml file find `<layoutList>`, then `<layout>` of your language, then `<variantList>` and paste the .xml snippet in-between other variants.

### Example
> ```
> <layout>
>   <configItem>
>     <name>us</name>
>     <!-- Keyboard indicator for English layouts -->
>     <shortDescription>en</shortDescription>
>     <description>English (US)</description>
>     <languageList>
>       <iso639Id>eng</iso639Id>
>     </languageList>
>   </configItem>
>   <variantList>
>     <!-- List of different US variants -->
>     <variant>
>       ...
>     </variant>
>     <YOUR SNIPPET HERE>
>     <variant>
>       ...
>     </variant>
>     ...
> ```
7. Log out (or restart your PC) and select your layout in your Settings or using `setxkbmap`.
