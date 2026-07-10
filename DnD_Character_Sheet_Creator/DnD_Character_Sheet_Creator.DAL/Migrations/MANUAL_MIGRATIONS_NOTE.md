Manual migration AddCharacterFieldsAndCharacterEquipment created on 2026-07-10.
This migration adds `CurrentExperiencePoints` and `DateOfLastLevelUp` to `Characters` and creates `CharacterEquipment` join table.
It attempts to migrate existing `Equipment.CharacterId` values into the join table and drops the `Equipment.CharacterId` column if present.

If you prefer auto-generated migrations, run:

    dotnet ef migrations add AddCharacterFieldsAndCharacterEquipment --project DnD_Character_Sheet_Creator.DAL --startup-project DnD_Character_Sheet_Creator.Web

Then inspect and run `dotnet ef database update`.
