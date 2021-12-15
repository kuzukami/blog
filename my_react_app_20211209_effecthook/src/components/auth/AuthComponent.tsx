import React, { useState, createContext } from 'react';

interface Props {
    children: React.ReactNode;
}

interface IauthContext {
    userId: string;
    setUserId: (name: string) => void;
}

const initializeAuthContext = {
    userId: "",
    setUserId: () =>{}
}

export const AuthContext = createContext<IauthContext>(initializeAuthContext);

const AuthComponent = (props: Props) => {
    const { children } = props;

    const [currentUser, setCurrentUser] = useState("nomurabbit");
    const signIn = (name: string) => {
        setCurrentUser(name);
    }
    
    return (
        <AuthContext.Provider value={{
            userId: currentUser,
            setUserId: signIn
        }}>
            {children}
        </AuthContext.Provider>
    );
}

export default AuthComponent