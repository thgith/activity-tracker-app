export interface ISession {
    id: any,
    activityId: any,
    startDateUtc: Date,
    durationSeconds: number,
    notes: string
};

export interface ISessionNew {
    activityId: any,
    startDate: Date,
    durationSeconds: number,
    notes: string
};

export interface ISessionEdit {
    id: string
    activityId: any,
    startDate: Date,
    durationSeconds: number,
    notes: string
};