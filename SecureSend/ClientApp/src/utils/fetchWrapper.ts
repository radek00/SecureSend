export const fetchWrapper = {
    get,
    post,
    put,
    delete: _delete
};

function get<T>(url: string): Promise<T> {
    const requestOptions = {
        method: 'GET'
    };
    return fetch(url, requestOptions).then(handleResponse<T>);
}

function post<T>(url: string, body?: Body, options?: any): Promise<T> {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };
    return fetch(url, options ?? requestOptions).then(handleResponse<T>);
}

function put<T>(url: string, body?: Body): Promise<T> {
    const requestOptions = {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };
    return fetch(url, requestOptions).then(handleResponse<T>);    
}

// prefixed with underscored because delete is a reserved word in javascript
function _delete<T>(url: string): Promise<T> {
    const requestOptions = {
        method: 'DELETE'
    };
    return fetch(url, requestOptions).then(handleResponse<T>);
}

// helper functions

function handleResponse<T>(response: Response): Promise<T> {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        
        if (!response.ok) {
            const error = (data && data.message) || response.statusText;
            return Promise.reject(new Error(error));
        }

        return data;
    });
}