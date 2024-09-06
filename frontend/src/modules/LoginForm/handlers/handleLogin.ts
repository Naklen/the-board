import { loginRequest } from '../API/loginRequest'

type HandleLoginType = {
    login: string,
    password: string
}

export async function handleLogin(credentials: HandleLoginType) {
    const accessTokenData = await loginRequest(credentials.login, credentials.password)
    localStorage.setItem('accessTokenData', accessTokenData.value)
}