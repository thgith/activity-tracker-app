using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Entities;

namespace ActivityTrackerAppTests.Fixtures;

public static class TestFixtures
{
    public const string PREHISTORIC_DATE_STR = "1022-07-13T01:27:26Z";
    public const string FUTURE_DATE_STR = "3022-07-13T01:27:26Z";
    public const string MIN_DATE_STR = "1/1/0001 12:00:00 AM";

    // NOTE: These are just random GUIDs
    public const string NON_EXISTENT_GUID_STR = "8068f966-91c6-4283-a8e4-279aa8b75d2b";
    // User GUID strings
    public const string JANE_USER_GUID_STR = "281b92b4-46e0-444f-aabe-da7aca9986a9";
    public const string JOHN_USER_GUID_STR = "e15d9f9a-4e5c-4311-b9c4-72cecdc2d672";
    public const string JUDY_USER_GUID_STR = "748d904a-84d4-4b81-9d69-23f5c7a9a746";

    // Activity GUID strings
    public const string PANIC_ACT_GUID_STR = "5bbf779e-acd0-4c35-be31-f4b67af0133f";
    public const string PIANO_ACT_GUID_STR = "de2e1178-0125-40ad-b6a3-f922a080489b";
    public const string GAME_DEV_ACT_GUID_STR = "3295ff98-14d1-4955-b4d0-97bed10c5833";
    public const string MCAT_ACT_GUID_STR = "b1f4699e-2694-4de4-bb85-200dc1fb94bd";
    public const string JOHNS_DELETED_ACT_GUID_STR = "c5e095cf-10b3-4924-9735-f7eed8903ceb";
    public const string BASEBALL_ACT_GUID_STR = "9c789a9a-1a20-4ec8-8375-0c2d3875dcd1";

    // Session GUID strings
    public const string PANIC_SESH_GUID_STR = "ad0c044c-2dc3-4eed-89bd-7011b737fd2c";
    public const string GAME_DEV_SESH1_GUID_STR = "c0a01773-4428-4c27-985f-1bd0be87712b";
    public const string GAME_DEV_SESH2_GUID_STR = "3f90adb9-95dc-4abe-bee6-13b1b2977763";
    public const string PIANO_SESH1_GUID_STR = "16f33737-8d96-405e-856a-a47d869b0cfc";
    public const string PIANO_SESH2_GUID_STR = "11d1ae01-841e-4239-a35c-7391c00a8328";
    public const string GAME_DEV_SESH_DELETED_GUID_STR = "f9f21b0e-888a-4a0c-827c-5c6ba40de3db";
    public const string JOHNS_DELETED_SESH_ON_DELETED_ACT_GUID_STR = "c9044a48-059c-42f7-82ac-bae88ee49afe";

    public static Guid NON_EXISTENT_GUID;

    // User
    // User GUIDs
    public static Guid JANE_USER_GUID;
    public static Guid JOHN_USER_GUID;
    public static Guid JUDY_USER_GUID;
    public static DateTime JANE_JOIN_DATE_UTC;
    public static DateTime JOHN_JOIN_DATE_UTC;
    public static DateTime JUDY_JOIN_DATE_UTC;
    public const string JANE_FIRST_NAME = "Jane";
    public const string JOHN_FIRST_NAME = "John";
    public const string JUDY_FIRST_NAME = "Judy";
    public const string LILA_FIRST_NAME = "Lila";
    public const string COMMON_LAST_NAME = "Doe";
    public const string JANE_EMAIL = "janedoe@test.com";
    public const string JOHN_EMAIL = "johndoe@test.com";
    public const string JUDY_EMAIL = "judydoe@test.com";
    public const string LILA_EMAIL = "liladoe@test.com";
    public const string COMMON_OLD_PASSWORD = "oldpassword";
    public const string NEW_FIRST_NAME = "Johnny";
    public const string NEW_LAST_NAME = "Deer";
    public const string NEW_EMAIL = "johnnydeer@test.com";
    public const string NEW_PASSWORD = "newpassword";

    // Activity
    // Activity GUIDs
    public static Guid PIANO_ACT_GUID;
    public static Guid GAME_DEV_ACT_GUID;
    public static Guid MCAT_ACT_GUID;
    public static Guid JOHNS_DELETED_ACT_GUID;
    public static Guid PANIC_ACT_GUID;
    public static Guid BASEBALL_ACT_GUID;
    public const string PIANO_ACTIVITY_NAME = "Piano";
    public const string PIANO_ACTIVITY_DESC = "Time spent practicing piano";
    public const string GAME_DEV_ACT_NAME = "Game Dev";
    public const string GAME_DEV_ACT_DESC = "Time spent on game development";
    public const string JOHNS_DELETED_ACT_NAME = "Sleeping";
    public const string MCAT_STUDY_ACTIVITY_NAME = "MCAT";
    public const string MCAT_STUDY_ACTIVITY_DESC = "Studying for the MCAT";
    public const string PANIC_ACTIVITY_NAME = "PANIC";
    public const string PANIC_ACTIVITY_DESC = "PANIC PANIC";
    public const string BASEBALL_ACTIVITY_NAME = "Baseball";
    public const string BASEBALL_ACTIVITY_DESC = "Playing baseball";
    // Tags
    public const string PERSONAL_TAG = "personal";
    public const string SCHOOL_TAG = "school";
    public const string PROGRAMMING_TAG = "programming";

    // Session
    // Session GUIDs
    public static Guid PANIC_SESH_GUID;
    public static Guid GAME_DEV_SESH1_GUID;
    public static Guid GAME_DEV_SESH2_GUID;
    public static Guid PIANO_SESH1_GUID;
    public static Guid PIANO_SESH2_GUID;
    public static Guid GAME_DEV_SESH_DELETED_GUID;
    public static Guid JOHNS_DELETED_SESH_ON_DELETED_ACT_GUID;

    public const string SHORT_GENERIC_NOTES = "notes notes...";
    public const string PIANO_SESH1_NOTES = "Scale exercises";
    public const string PIANO_SESH2_NOTES = "Learning new song";
    public const string GAME_DEV_SESH1_NOTES = "Game jam";
    public const string GAME_DEV_SESH2_NOTES = "More game jam";

    public static User GenerateJaneUser()
    {
        return new User
        {
            Id = JANE_USER_GUID,
            FirstName = JANE_FIRST_NAME,
            LastName = COMMON_LAST_NAME,
            Email = JANE_EMAIL,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(COMMON_OLD_PASSWORD),
            JoinDateUtc = JANE_JOIN_DATE_UTC,
            DeletedDateUtc = null,
            Role = Roles.ADMIN
        };
    }

    public static User GenerateJohnUser()
    {
        return new User
        {
            Id = JOHN_USER_GUID,
            FirstName = JOHN_FIRST_NAME,
            LastName = COMMON_LAST_NAME,
            Email = JOHN_EMAIL,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(COMMON_OLD_PASSWORD),
            // To make this user's join date later than the first user's
            JoinDateUtc = JOHN_JOIN_DATE_UTC,
            DeletedDateUtc = null,
            Role = Roles.MEMBER
        };
    }

    public static User GenerateJudyUser()
    {
        return new User
        {
            Id = JUDY_USER_GUID,
            FirstName = JUDY_FIRST_NAME,
            LastName = COMMON_LAST_NAME,
            Email = JUDY_EMAIL,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(COMMON_OLD_PASSWORD),
            JoinDateUtc = JUDY_JOIN_DATE_UTC,
            DeletedDateUtc = DateTime.UtcNow,
            Role = Roles.MEMBER
        };
    }

    public static Activity GeneratePanicActivity()
    {
        return new Activity
        {
            Id = PANIC_ACT_GUID,
            OwnerId = JANE_USER_GUID,
            Name = PANIC_ACTIVITY_NAME,
            Description = PANIC_ACTIVITY_DESC,
            StartDateUtc = DateTime.UtcNow,
            DueDateUtc = null,
            CompletedDateUtc = null,
            DeletedDateUtc = null,
            IsArchived = false,
            ColorHex = null,
            Tags = null
        };
    }

    public static Activity GenerateGameDevActivity()
    {
        return new Activity
        {
            Id = GAME_DEV_ACT_GUID,
            OwnerId = JOHN_USER_GUID,
            Name = GAME_DEV_ACT_NAME,
            Description = GAME_DEV_ACT_DESC,
            StartDateUtc = DateTime.UtcNow,
            DueDateUtc = null,
            CompletedDateUtc = null,
            DeletedDateUtc = null,
            IsArchived = false,
            ColorHex = null,
            Tags = new List<string> { PERSONAL_TAG, PROGRAMMING_TAG }
        };
    }
    public static Activity GeneratePianoActivity()
    {
        return new Activity
        {
            Id = PIANO_ACT_GUID,
            OwnerId = JOHN_USER_GUID,
            Name = GAME_DEV_ACT_NAME,
            Description = GAME_DEV_ACT_DESC,
            StartDateUtc = DateTime.UtcNow.AddSeconds(1),
            DueDateUtc = null,
            CompletedDateUtc = null,
            DeletedDateUtc = null,
            IsArchived = false,
            ColorHex = "#0000ff",
            Tags = new List<string> { PERSONAL_TAG }
        };
    }
    public static Activity GenerateMcatActivity()
    {
        return new Activity
        {
            Id = MCAT_ACT_GUID,
            OwnerId = JOHN_USER_GUID,
            Name = MCAT_STUDY_ACTIVITY_NAME,
            Description = MCAT_STUDY_ACTIVITY_DESC,
            StartDateUtc = DateTime.UtcNow.AddSeconds(2),
            DueDateUtc = DateTime.UtcNow.AddMonths(2),
            CompletedDateUtc = null,
            IsArchived = false,
            DeletedDateUtc = null,
            ColorHex = "#ff0000",
            Tags = new List<string> { SCHOOL_TAG }
        };
    }
    // Deleted activity
    public static Activity GenerateJohnsDeletedActivity()
    {
        return new Activity
        {
            Id = JOHNS_DELETED_ACT_GUID,
            OwnerId = JOHN_USER_GUID,
            Name = JOHNS_DELETED_ACT_NAME,
            Description = "",
            StartDateUtc = DateTime.UtcNow,
            DueDateUtc = null,
            CompletedDateUtc = null,
            DeletedDateUtc = DateTime.UtcNow,
            IsArchived = false,
            ColorHex = null,
            Tags = null
        };
    }

    // Belongs to a deleted user, so this is also deleted
    public static Activity GenerateBaseballActivity()
    {
        return new Activity
        {
            Id = BASEBALL_ACT_GUID,
            OwnerId = JUDY_USER_GUID,
            Name = BASEBALL_ACTIVITY_NAME,
            Description = BASEBALL_ACTIVITY_DESC,
            StartDateUtc = DateTime.UtcNow,
            DueDateUtc = null,
            CompletedDateUtc = null,
            // When user is deleted, activity is also deleted
            DeletedDateUtc = DateTime.UtcNow,
            IsArchived = true,
            ColorHex = null,
            Tags = null,
        };
    }

    // Sessions
    public static Session GeneratePanicSession()
    {
        return new Session
        {
            Id = PANIC_SESH_GUID,
            ActivityId = PANIC_ACT_GUID,
            Notes = "",
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 2000,
            DeletedDateUtc = null
        };
    }

    public static Session GenerateGameDevSession1()
    {
        return new Session
        {
            Id = GAME_DEV_SESH1_GUID,
            ActivityId = GAME_DEV_ACT_GUID,
            Notes = GAME_DEV_SESH1_NOTES,
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 3600,
            DeletedDateUtc = null
        };
    }

    public static Session GenerateGameDevSession2()
    {
        return new Session
        {
            Id = GAME_DEV_SESH2_GUID,
            ActivityId = GAME_DEV_ACT_GUID,
            Notes = GAME_DEV_SESH2_NOTES,
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 4600,
            DeletedDateUtc = null
        };
    }

    public static Session GenerateGameDevDeletedSession()
    {
        return new Session
        {
            Id = GAME_DEV_SESH2_GUID,
            ActivityId = GAME_DEV_ACT_GUID,
            Notes = GAME_DEV_SESH2_NOTES,
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 4600,
            DeletedDateUtc = DateTime.UtcNow
        };
    }

    public static Session GeneratePianoSession1()
    {
        return new Session
        {
            Id = PIANO_SESH1_GUID,
            ActivityId = PIANO_ACT_GUID,
            Notes = PIANO_SESH1_NOTES,
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 6600,
            DeletedDateUtc = null
        };
    }

    public static Session GeneratePianoSession2()
    {
        return new Session
        {
            Id = PIANO_SESH2_GUID,
            ActivityId = PIANO_ACT_GUID,
            Notes = PIANO_SESH2_NOTES,
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 2000,
            DeletedDateUtc = null
        };
    }

    static TestFixtures()
    {
        Guid.TryParse(NON_EXISTENT_GUID_STR, out NON_EXISTENT_GUID);

        Guid.TryParse(JANE_USER_GUID_STR, out JANE_USER_GUID);
        Guid.TryParse(JOHN_USER_GUID_STR, out JOHN_USER_GUID);
        Guid.TryParse(JUDY_USER_GUID_STR, out JUDY_USER_GUID);

        // Activity GUIDs
        Guid.TryParse(PANIC_ACT_GUID_STR, out PANIC_ACT_GUID);
        Guid.TryParse(PIANO_ACT_GUID_STR, out PIANO_ACT_GUID);
        Guid.TryParse(GAME_DEV_ACT_GUID_STR, out GAME_DEV_ACT_GUID);
        Guid.TryParse(MCAT_ACT_GUID_STR, out MCAT_ACT_GUID);
        Guid.TryParse(JOHNS_DELETED_ACT_GUID_STR, out JOHNS_DELETED_ACT_GUID);
        Guid.TryParse(BASEBALL_ACT_GUID_STR, out BASEBALL_ACT_GUID);

        // Session GUIDs
        Guid.TryParse(PANIC_SESH_GUID_STR, out PANIC_SESH_GUID);
        Guid.TryParse(GAME_DEV_SESH1_GUID_STR, out GAME_DEV_SESH1_GUID);
        Guid.TryParse(GAME_DEV_SESH2_GUID_STR, out GAME_DEV_SESH2_GUID);
        Guid.TryParse(GAME_DEV_SESH_DELETED_GUID_STR, out GAME_DEV_SESH_DELETED_GUID);
        Guid.TryParse(PIANO_SESH1_GUID_STR, out PIANO_SESH1_GUID);
        Guid.TryParse(PIANO_SESH2_GUID_STR, out PIANO_SESH2_GUID);
        Guid.TryParse(JOHNS_DELETED_SESH_ON_DELETED_ACT_GUID_STR, out GAME_DEV_SESH_DELETED_GUID);

        JANE_JOIN_DATE_UTC = DateTime.UtcNow;
        JOHN_JOIN_DATE_UTC = DateTime.UtcNow.AddSeconds(1);
        JUDY_JOIN_DATE_UTC = DateTime.UtcNow.AddSeconds(2);
    }
}