import requests


class MicroGPT:

    def __init__(self, base_url: str):
        self.base_url = base_url

    def models(self):
        response = requests.get(self.base_url + "/v1/models").json()
        return response["data"]

    def completions(self, system_prompt: str, user_prompt: str,
                    temperature: float = 0.8):
        payload = {"messages": [
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_prompt}
        ],
            "temperature": temperature,
            "stream": False
        }
        response = requests.post(self.base_url + "/v1/chat/completions", json=payload).json()
        return response
