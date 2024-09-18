import { loginRequest } from '../API/loginRequest'

type HandleLoginType = {
    email: string,
    password: string
}

export async function handleLogin(credentials: HandleLoginType) {
    const accessTokenData = await loginRequest(credentials.email, credentials.password)
    localStorage.setItem('accessTokenData', accessTokenData.value)
}