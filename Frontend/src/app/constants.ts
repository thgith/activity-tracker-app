export const REQUIRED_FIELD_MSG = 'This field is required.';
export const SHORT_TEXT_MAX_CHAR = 50;
export const LONG_TEXT_MAX_CHAR = 1024;
export const API_URL_BASE = 'https://localhost:7109/api/v1/';
// NOTE: This is a non http-only cookie that doesn't store any JWT info.
//       It's just for logic after the user is already authenticated with
//       with a JWT token stored elsewhere
export const CURR_USER_ID_COOKIE_NAME = 'curr_user_id';

export const STANDARD_DATE_DISPLAY_FORMAT = 'MMMM DD, YYYY';
export const LONG_DATE_FORMAT = 'dddd MM/DD/YYYY hh:mm A';
export const PICKER_DATE_DISPLAY_FORMAT = 'YYYY-MM-DD';

// NOTE: This is used to indicate a date time that has been reset by the user
//       We need a min date to indicate this because null means the user didn't touch the field
//       while this means they manually cleared the field.
export const MIN_DATE = new Date('0001-01-01T00:00:00Z');

export const LIGHT_GREY = '#f8f9fa';
export const MED_GREY = '#dee2e6';
export const DARK_GREY = '#6c757d';
export const YELLOW = '#Ffcc33';
export const ORANGE = '#Ff9933';
export const CORAL = '#Ff6666';
export const PINK = '#ff66cc';
export const PURPLE = '#9966ff';
export const BLUE = '#3366ff';
export const BLUE_GREEN = '#00cccc';
export const GREEN = '#00cc66';
export const DEFAULT_COLOR = BLUE_GREEN;