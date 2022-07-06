export interface ISession {
    id: any,
    activityId: any,
    startDateUtc: Date,
    durationSeconds: number,
    notes: string
}

export interface ISessionNew {
    activityId: any,
    startDateUtc: Date,
    durationSeconds: number,
    notes: string
}