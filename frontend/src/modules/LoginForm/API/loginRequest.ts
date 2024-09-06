type ResponseBodyType = {
    value: string;
};

export async function loginRequest(
    login: string,
    password: string
): Promise<ResponseBodyType> {
    const response = await fetch(
        process.env.NEXT_PUBLIC_BACKEND_API_URL + "/user/login",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json;charset=utf-8",
            },
            body: JSON.stringify({ login, password }),
            credentials: "include",
        }
    );
    return await response.json();
}
