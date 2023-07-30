import { ErrorTypes } from "@/models/enums/ErrorTypes";
import {
  UploadDoesNotExistError,
  UploadExpiredError,
} from "@/models/errors/ResponseErrors";

interface IErrorResponse {
  Message: string;
  ErrorCode: ErrorTypes;
}
export const fetchWrapper = {
  get,
  post,
  put,
  delete: _delete,
};

function get<T>(url: string): Promise<T> {
  const requestOptions = {
    method: "GET",
  };
  return fetch(url, requestOptions).then(handleResponse<T>);
}

function post<T>(url: string, body?: Body, options?: RequestInit): Promise<T> {
  const requestOptions = {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  };
  return fetch(url, options ?? requestOptions).then(handleResponse<T>);
}

function put<T>(url: string, body?: Body): Promise<T> {
  const requestOptions = {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  };
  return fetch(url, requestOptions).then(handleResponse<T>);
}

// prefixed with underscored because delete is a reserved word in javascript
function _delete<T>(url: string): Promise<T> {
  const requestOptions = {
    method: "DELETE",
  };
  return fetch(url, requestOptions).then(handleResponse<T>);
}

// helper functions

async function handleResponse<T>(response: Response): Promise<T> {
  return response.text().then((text) => {
    const data = text && JSON.parse(text);

    if (!response.ok) {
      if (data) {
        if ((data as IErrorResponse).ErrorCode === ErrorTypes.upload_expired)
          return Promise.reject(new UploadExpiredError(data.Message));
        if (
          (data as IErrorResponse).ErrorCode ===
          ErrorTypes.upload_does_not_exist
        )
          return Promise.reject(new UploadDoesNotExistError(data.Message));
      }
      return Promise.reject(new Error(response.statusText));
    }

    return data;
  });
}
