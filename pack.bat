SET cur_dir=%~dp0
SET output=%cur_dir%.build

SET build_cmd=dotnet pack -c Release --output %output%
%build_cmd% %cur_dir%cmstar.Data
