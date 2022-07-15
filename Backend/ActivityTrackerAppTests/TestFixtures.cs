using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Entities;

namespace ActivityTrackerAppTests.Fixtures;

public static class TestFixtures
{
    static TestFixtures()
    {
        // NOTE: These are just random GUIDs
        // User GUIDs
        Guid.TryParse("281b92b4-46e0-444f-aabe-da7aca9986a9", out JANE_USER_GUID);
        Guid.TryParse("e15d9f9a-4e5c-4311-b9c4-72cecdc2d672", out JOHN_USER_GUID);
        Guid.TryParse("748d904a-84d4-4b81-9d69-23f5c7a9a746", out JUDY_USER_GUID);

        // Activity GUIDs
        Guid.TryParse("5bbf779e-acd0-4c35-be31-f4b67af0133f", out PANIC_ACT_GUID);
        Guid.TryParse("de2e1178-0125-40ad-b6a3-f922a080489b", out PIANO_ACT_GUID);
        Guid.TryParse("3295ff98-14d1-4955-b4d0-97bed10c5833", out GAME_DEV_ACT_GUID);
        Guid.TryParse("b1f4699e-2694-4de4-bb85-200dc1fb94bd", out MCAT_ACT_GUID);
        Guid.TryParse("c5e095cf-10b3-4924-9735-f7eed8903ceb", out SLEEPING_ACT_GUID);
        Guid.TryParse("9c789a9a-1a20-4ec8-8375-0c2d3875dcd1", out BASEBALL_ACT_GUID);

        // Session GUIDs
        Guid.TryParse("16f33737-8d96-405e-856a-a47d869b0cfc", out PIANO_SESH1_GUID);
        Guid.TryParse("11d1ae01-841e-4239-a35c-7391c00a8328", out PIANO_SESH2_GUID);
        Guid.TryParse("c0a01773-4428-4c27-985f-1bd0be87712b", out GAME_DEV_SESH1_GUID);
        Guid.TryParse("3f90adb9-95dc-4abe-bee6-13b1b2977763", out GAME_DEV_SESH2_GUID);

        JANE_JOIN_DATE_UTC = DateTime.UtcNow;
        JOHN_JOIN_DATE_UTC = DateTime.UtcNow.AddSeconds(1);
        JUDY_JOIN_DATE_UTC = DateTime.UtcNow.AddSeconds(2);
    }

    public static DateTime JANE_JOIN_DATE_UTC;
    public static DateTime JOHN_JOIN_DATE_UTC;
    public static DateTime JUDY_JOIN_DATE_UTC;

    public static bool DatesEqualWithinSeconds(DateTime date, DateTime laterDate, int seconds = 60)
    {
        TimeSpan timeSpan = laterDate.Subtract(date);
        return timeSpan.TotalMinutes < seconds;
    }

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
            Tags = new List<string>{ PERSONAL_TAG, PROGRAMMING_TAG }
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
            Tags = new List<string>{ PERSONAL_TAG }
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
            Tags = new List<string>{ SCHOOL_TAG }
        };
    }
    // Deleted activity
    public static Activity GenerateSleepingActivity()
    {
        return new Activity
        {
            Id = SLEEPING_ACT_GUID,
            OwnerId = JOHN_USER_GUID,
            Name = SLEEPING_ACT_NAME,
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
    public static Session GeneratePianoSession1()
    {
        return new Session
        {
            Id = PIANO_SESH1_GUID,
            ActivityId = PIANO_ACT_GUID,
            Notes = PIANO_SESH1_NOTES,
            StartDateUtc = DateTime.UtcNow,
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
            DeletedDateUtc = null
        };
    }

    // User
    public static Guid JANE_USER_GUID;
    public static Guid JOHN_USER_GUID;
    public static Guid JUDY_USER_GUID;
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
    public static Guid SLEEPING_ACT_GUID;
    public static Guid PANIC_ACT_GUID;
    public static Guid BASEBALL_ACT_GUID;
    public const string PIANO_ACTIVITY_NAME = "Piano";
    public const string PIANO_ACTIVITY_DESC = "Time spent practicing piano";
    public const string GAME_DEV_ACT_NAME = "Game Dev";
    public const string GAME_DEV_ACT_DESC = "Time spent on game development";
    public const string SLEEPING_ACT_NAME = "Sleeping";
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
    public static Guid PIANO_SESH1_GUID;
    public static Guid PIANO_SESH2_GUID;
    public static Guid GAME_DEV_SESH1_GUID;
    public static Guid GAME_DEV_SESH2_GUID;

    public const string PIANO_SESH1_NOTES = "Scale exercises";
    public const string PIANO_SESH2_NOTES = "Learning new song";
    public const string GAME_DEV_SESH1_NOTES = "Game jam";
    public const string GAME_DEV_SESH2_NOTES = "More game jam";
}