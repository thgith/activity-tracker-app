export interface IActivity {
    id: string?,
    name: string?,
    description: string?,
    // NOTE: We check empty string in case the user, cancels out of the date field.
    startDateUtc: Date | string | null,
    dueDateUtc: Date | string | null,
    completedDateUtc: Date | string | null,
    isArchived: boolean?,
    colorHex: string?,
    tags: string[] | null,
    sessions: ISession[]
};

export interface IActivityEdit {
    name: string?,
    description: string?,
    // NOTE: We check empty string in case the user, cancels out of the date field.
    startDate: Date | string | null,
    dueDate: Date | string | null,
    completedDate: Date | string | null,
    isArchived: boolean?,
    colorHex: string?,
    tags: string[] | null
};