const Request = window.Request;
const Headers = window.Headers;
const fetch = window.fetch;

class Domain {
  constructor({ content, success, message }) {
    this.success = success;
    this.message = message;

    this.name = content && content.name;
    this.ip = content && content.ip;
    this.whoIs = content && content.whoIs;
    this.hostedAt = content && content.hostedAt;
  }

  static isNameValid(domainName) {
    return domainName.match(/^((?!-))(xn--)?[a-z0-9][a-z0-9-_]{0,61}[a-z0-9]{0,1}\.(xn--)?([a-z0-9\-]{1,61}|[a-z0-9-]{1,30}\.[a-z]{2,})$/);
  }

  static async search(domainName) {
    if (!domainName || !this.isNameValid(domainName)) return dom.setAlertBox('Digite um nome de domínio válido.');

    dom.disableCmps();

    return await api.getDomain(domainName)
      .then(response => {
        dom.enableCmps();

        return new Domain(response);
      });
  }
}

class Api {
  async request(method, url, body) {
    const request = new Request('/api/' + url, {
      method: method,
      body: body && JSON.stringify(body),
      credentials: 'same-origin',
      headers: new Headers({
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      })
    });

    const resp = await fetch(request);

    const jsonResult = await resp.json();

    if (!resp.ok) {
      return { content: null, status: resp.status, success: resp.ok, message: jsonResult };
    }

    return { content: jsonResult, status: resp.status, success: resp.ok, message: null };
  }

  async getDomain(domainOrIp) {
    const response = await this.request('GET', `domain/${domainOrIp}`);

    return { content: response.content, success: response.success, message: response.message };
  }
}

class Dom {
  constructor() {
    this.btnSearch = document.getElementById('btn-search');
    this.domainInput = document.getElementById('txt-search');
    this.alertBox = document.getElementById('alert-box');
    this.results = document.getElementById('results');
    this.ip = document.getElementById('ip-data');
    this.domain = document.getElementById('domain-data');
    this.hostedAt = document.getElementById('hostedat-data');
    this.whois = document.getElementById('whois-data');

    this.cleanDom();
  }

  cleanDom() {
    this.enableCmps();
    this.invisibleResults();
    this.hideAlertBox();
  }

  getDomainInputValue() {
    return this.domainInput.value;
  }

  getBtnSearch() {
    return this.btnSearch;
  }

  setResults(domain) {
    if (!domain.success) return this.setAlertBox(domain.message);

    this.visibleResults();
    this.setDomainResults(domain);
  }

  setDomainResults(domain) {
    this.ip.innerHTML = domain.ip;
    this.domain.innerHTML = domain.name;
    this.hostedAt.innerHTML = domain.hostedAt;
    this.whois.innerHTML = domain.whoIs.split('\n').join('<br/>');
  }

  disableCmps() {
    this.disableBtnSearch();
    this.disableInput();
  }

  enableCmps() {
    this.enableBtnSearch();
    this.enableInput();
  }

  setAlertBox(message) {
    this.showAlertBox();

    this.alertBox.innerHTML = message;
  }

  hideAlertBox() {
    this.alertBox.style.display = 'none';
  }

  showAlertBox() {
    this.alertBox.style.display = 'block';
  }

  visibleResults() {
    this.results.style.visibility = 'visible';
  }

  invisibleResults() {
    this.results.style.visibility = 'hidden';
  }

  disableBtnSearch() {
    this.btnSearch.disabled = true;
  }

  enableBtnSearch() {
    this.btnSearch.disabled = false;
  }

  disableInput() {
    this.domainInput.disabled = true;
  }

  enableInput() {
    this.domainInput.disabled = false;
  }
}

const api = new Api();
const dom = new Dom();

var callback = () => {
  if (!dom.getBtnSearch()) return;

  dom.getBtnSearch().onclick = async () => {
    dom.cleanDom();

    const domainName = dom.getDomainInputValue();

    const response = await Domain.search(domainName);

    if (!response) return this.setAlertBox('Não foi possível verificar o domínio.');;

    dom.setResults(response);
  }
}

if (document.readyState === 'complete' || (document.readyState !== 'loading' && !document.documentElement.doScroll)) {
  callback();
} else {
  document.addEventListener('DOMContentLoaded', callback);
}
