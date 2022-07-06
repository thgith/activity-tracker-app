export interface IUser {
    id: any,
    firstName: string,
    lastName: string,
    email: string,
    joinDateUtc: Date,
    role: string
};

export interface IUserUpdate {
    id: any,
    firstName: string,
    lastName: string,
    email: string,
};

export interface IRegister {
    firstName: string,
    lastName: string,
    email: string,
    password: string
};

export interface ILogin {
    email: string,
    password: string
}